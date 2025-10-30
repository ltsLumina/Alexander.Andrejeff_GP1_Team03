#region
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

public class InputManager : MonoBehaviour
{
	[SerializeField, ReadOnly] Vector2 moveInput;

	public Vector2 MoveInput => moveInput;

	[field: SerializeField]
	public bool IsSprinting { get; private set; }

	PlayerController player;

	void Awake() { player = GetComponentInParent<PlayerController>(); }

	public void OnMove(InputAction.CallbackContext context)
	{
		moveInput = context.ReadValue<Vector2>();

		//Logger.Log($"Move Input: {moveInput}", this, "InputManager");
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		if (context.performed) { player?.Weapon.Attack(); }
	}

	bool kickHeld;

	public void OnKick(InputAction.CallbackContext context)
	{
		if (context.performed) { player?.Weapon.Kick(); }
	}

	public void OnInteract(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Ray ray = Helpers.CameraMain.ViewportPointToRay(new Vector3(0.5f, 0f, 0f));

			if (Physics.Raycast(ray, out RaycastHit hitInfo, 3f))
			{
				hitInfo.collider.TryGetComponent(out IInteractable interactable);
				interactable?.Interact();
			}

			Debug.DrawRay(ray.origin, ray.direction * 3f, Color.green, 1f);
		}
	}

	public void OnSprint(InputAction.CallbackContext context)
	{
		// not currently used, or maybe ever
	}
}
