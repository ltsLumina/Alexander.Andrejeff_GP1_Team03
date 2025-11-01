#region
using System;
using Abiogenesis3d;
using JetBrains.Annotations;
using Lumina.Essentials.Modules;
using MelenitasDev.SoundsGood;
using UnityEditor;
using UnityEngine;
using VInspector;
#endregion

[SelectionBase, DisallowMultipleComponent, RequireComponent(typeof(CharacterController))] 
public class PlayerController : MonoBehaviour, IDamageable
{
	[Tab("Movement")]
	[SerializeField] float baseMoveSpeed = 5f;
	[SerializeField] int moveWindingTime = 50;
	[SerializeField] float movementAddmult = -0.5f;

	[Tab("Camera")]
	[SerializeField] float baseCameraRotateSpeed = 240f;
	[Tooltip("Higher values mean slower winding up/down.")]
	[SerializeField] int cameraWindingTime = 16;
	[Tooltip("Controls the added multiplier for winding (negative values reduce speed).")]
	[SerializeField] float cameraWindAddMultiplier = -0.8f;

	[Tab("Weapon")]
	[SerializeField] Weapon weapon;

	[Tab("Settings")]
	[SerializeField] bool debugMode;
	[SerializeField] bool disablePixelization;
	[Button, ShowIf(nameof(debugMode))]
	[UsedImplicitly]
	void Kill() => TakeDamage(999);
	[EndIf]
	
	float rotateCamera;
	int cameraWindingCounter;
	float cameraWindingCurrentMult;
	float prevCameraDirection;

	float movePlayer;
	int moveWindingCounter;
	float moveWindingCurrentMult;
	float prevMoveDirection;

	// references

	CharacterController controller;
	InputManager inputs;
	PlayerHealth healthComponent;

	public Weapon Weapon => weapon;

	Sound footstepsCement;

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (controller != null)
		{
			Gizmos.color = Color.green;
			Handles.Label(transform.position + Vector3.up * 2f, $"Velocity: {controller.velocity.magnitude:F2}");
			Gizmos.DrawLine(transform.position, transform.position + controller.velocity);
		}
	}

	void OnValidate()
	{
		var pixelator = GetComponentInChildren<UPixelator>();
		if (pixelator != null) pixelator.enabled = !disablePixelization;
	}
#endif

	void Awake()
	{
		controller = GetComponent<CharacterController>();
		inputs = GetComponentInChildren<InputManager>();
		healthComponent = GetComponent<PlayerHealth>();
	}

	void Start()
	{
		footstepsCement = new Sound(SFX.FootstepsCement);
		footstepsCement.SetLoop(true);
		footstepsCement.SetVolume(0.25f);
		footstepsCement.SetSpatialSound();
		footstepsCement.SetFollowTarget(transform);
		footstepsCement.Play();
		
#if UNITY_EDITOR
		var gameViewType = Type.GetType("UnityEditor.GameView,UnityEditor");
		var gameViewWindow = EditorWindow.GetWindow(gameViewType);

		Debug.Assert(gameViewWindow != null, "Game View window not found! Cannot set pixelation!");
#endif
	}

	void Update()
	{
		rotateCamera = Mathf.RoundToInt(inputs.MoveInput.x);
		movePlayer = Mathf.RoundToInt(inputs.MoveInput.y);
		
		if (Math.Abs(movePlayer) > 0.01f)
		{
			if (!footstepsCement.Playing) footstepsCement.Resume();
		}
		else
		{
			if (footstepsCement.Playing) footstepsCement.Pause();
		}
		
	}

	void FixedUpdate()
	{
		// Multiplies movement speed
		if ((movePlayer == 0) | (Math.Abs(prevMoveDirection - movePlayer) > 0.01f)) // review: floating-point precision loss prevention
		{
			moveWindingCounter = moveWindingTime;
			prevMoveDirection = movePlayer;
		}
		else if (moveWindingCounter != 0) { moveWindingCounter -= 1; }

		// Multiplies camera rotation speed
		if ((rotateCamera == 0) | (Math.Abs(prevCameraDirection - rotateCamera) > 0.01f)) // review: floating-point precision loss prevention
		{
			cameraWindingCounter = cameraWindingTime;
			prevCameraDirection = rotateCamera;
		}
		else if (cameraWindingCounter != 0) { cameraWindingCounter -= 1; }

		moveWindingCurrentMult = 1f + movementAddmult * moveWindingCounter / moveWindingTime;
		cameraWindingCurrentMult = 1f + cameraWindAddMultiplier * cameraWindingCounter / cameraWindingTime;

		Vector3 playerMove = transform.forward * (movePlayer * moveWindingCurrentMult * baseMoveSpeed);
		Quaternion playerRotation = Quaternion.Euler(Vector3.up * (rotateCamera * baseCameraRotateSpeed * cameraWindingCurrentMult * Time.fixedDeltaTime));

		controller.SimpleMove(playerMove);
		transform.Rotate(playerRotation.eulerAngles);
	}

	public void TakeDamage(float damage) // Helper function to pass damage to PlayerHealth component
	{
		healthComponent.TakeDamage(damage);

		//Weapon.Attack();
		//Weapon.Kick();
	}
}
