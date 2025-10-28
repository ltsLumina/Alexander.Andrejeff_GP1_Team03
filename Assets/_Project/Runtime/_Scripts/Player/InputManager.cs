using System;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField, ReadOnly] Vector2 moveInput;

    public Vector2 MoveInput => moveInput;

    PlayerController player;
    
    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Logger.Log($"Move Input: {moveInput}", this, "InputManager");
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            player?.Weapon.Attack();
        }
    }

    public void OnInteract(InputAction.CallbackContext context) { }

    public void OnSprint(InputAction.CallbackContext context) { }

    //public void OnKick(InputAction.CallbackContext context) { } // same as interact, might change in the future.
}
