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
	[SerializeField] float gravityMult = 1.5f;

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
	public float BaseMoveSpeed
	{
		get => baseMoveSpeed;
		set => baseMoveSpeed = value;
	}

	Sound footstepsCement;
	Music fallingSFX;
	Sound hurtSFX;

	public bool HasRelic { get; set; }

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

	void OnDrawGizmosSelected()
	{
		if (!Application.isPlaying) return;
		Gizmos.color = IsGrounded ? Color.blue : Color.red;
		Gizmos.DrawWireSphere(transform.position + Vector3.down * (controller.height / 2f + 0.1f), controller.radius);
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
		footstepsCement.SetOutput(Output.SFX);
		footstepsCement.SetLoop(true);
		footstepsCement.SetVolume(0.25f);
		footstepsCement.SetSpatialSound();
		footstepsCement.SetFollowTarget(transform);
		footstepsCement.Play();

		fallingSFX = new Music(Track.FallingSlow);
		fallingSFX.SetOutput(Output.SFX);
		fallingSFX.SetVolume(0.5f);
		fallingSFX.SetFollowTarget(transform);

		hurtSFX = new Sound(SFX.Minecraft);
		hurtSFX.SetOutput(Output.SFX);
		hurtSFX.SetVolume(0.7f);
		hurtSFX.SetFollowTarget(transform);
		
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
		
		if (!IsGrounded)
		{
			if (!fallingSFX.Playing && fallingFrameCounter > 30) fallingSFX.Play(0.5f);
			controller.Move(Vector3.down * (9.81f * gravityMult * Time.fixedDeltaTime));
		}
		else
		{
			fallingSFX.Stop(1f);
			controller.SimpleMove(playerMove);
		}

		transform.Rotate(playerRotation.eulerAngles);
	}

	public bool IsGrounded
	{
		get
		{
			bool grounded = Physics.SphereCast(transform.position, controller.radius, Vector3.down, out RaycastHit hit, controller.height / 2f + 0.1f, LayerMask.GetMask("Ground"));
			fallingFrameCounter = grounded ? 0 : fallingFrameCounter + 1;
			return grounded;
		}
	}
	
	public bool IsFalling => fallingFrameCounter > 30;
	
	int fallingFrameCounter;

	public void TakeDamage(float damage, DamageSource source = DamageSource.Player) // Helper function to pass damage to PlayerHealth component
	{
		healthComponent.TakeDamage(damage);
		hurtSFX.Play();
	}
}
