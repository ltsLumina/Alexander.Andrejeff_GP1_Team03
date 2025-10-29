#region
using System;
using JetBrains.Annotations;
using UnityEngine;
using VInspector;
#endregion

[SelectionBase, DisallowMultipleComponent, RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamageable
{
	[Tab("Movement")]
	[SerializeField] float movementSpeed = 3f;
	[SerializeField] float cameraRotateSpeed = 180f;
	
	[Tab("Weapon")]
	[SerializeField] Weapon weapon;

	[Tab("Settings")]
	[SerializeField] bool debugMode;
	[Button, ShowIf(nameof(debugMode))]
	[UsedImplicitly]
	void Kill() => Death();
	[EndIf]

	float rotateCamera;
	float movePlayer;
	
	// references
	
	CharacterController controller;
	Rigidbody rb;
	InputManager inputs;

	public Weapon Weapon => weapon;

	void Awake()
	{
		controller = GetComponent<CharacterController>();
		inputs = GetComponentInChildren<InputManager>();
	}

	void Update()
	{
		rotateCamera = inputs.MoveInput.x;
		movePlayer = inputs.MoveInput.y;
	}

	void FixedUpdate()
	{
		Vector3 playerMove = transform.forward * (movePlayer * movementSpeed);
		Quaternion playerRotation = Quaternion.Euler(Vector3.up * (rotateCamera * cameraRotateSpeed * Time.fixedDeltaTime));

		controller.SimpleMove(playerMove);
		transform.Rotate(playerRotation.eulerAngles); // TODO: Make this rotate quickly at the start, so its easier to quickly turn ~45 degrees
														// P.S. we also need aim assist towards enemies
	}

	public void TakeDamage(float damage) => throw new NotImplementedException();

	void Death() => throw new NotImplementedException();
}
