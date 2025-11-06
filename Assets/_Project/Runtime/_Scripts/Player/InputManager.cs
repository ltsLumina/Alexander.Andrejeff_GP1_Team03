#region
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

public class InputManager : MonoBehaviour
{
    [SerializeField] List<DirectionalCheckpoint> checkpointList;
    Queue<DirectionalCheckpoint> checkpoints;

    [SerializeField] BreadcrumbTrail breadcrumbPrefab;
    [SerializeField] Transform breadcrumbStartPoint;
    [SerializeField] Transform breadcrumbTarget;    

    enum InputMode
    {
        Standard,
        OneHanded
    }

    [SerializeField, ReadOnly] Vector2 moveInput;
    [SerializeField] InputMode inputMode;

    public Vector2 MoveInput => moveInput;

    PlayerController player;

    void Awake()
    {
        checkpoints = new Queue<DirectionalCheckpoint>(checkpointList);
        player = GetComponentInParent<PlayerController>();
    }

    void Start() => inputMode = PlayerPrefs.GetInt("OneHandedSetting", 0) == 1 ? InputMode.OneHanded : InputMode.Standard;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (inputMode == InputMode.OneHanded) moveInput = new(-moveInput.y, moveInput.x);
    }

    void OnEnable() => DirectionalCheckpointsReigstry.OnNewCheckpointTarget += HandleDirectionalCheckpointReached;
    void OnDisable() => DirectionalCheckpointsReigstry.OnNewCheckpointTarget -= HandleDirectionalCheckpointReached;

    public void HandleDirectionalCheckpointReached(Transform target)
    {
        breadcrumbTarget = target;
    }

    bool attackHeld;
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (player.Weapon.EquippedWeapon)
            {
                case Weapon.Weapons.Staff:
                    break;

                case Weapon.Weapons.Dagger:
                    player.Weapon.StartCoroutine(player.Weapon.Attack());
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (context.started)
        {
            attackHeld = true;
        }

        if (context.canceled)
        {
            attackHeld = false;
        }
    }

    bool kickHeld;

    public void OnKick(InputAction.CallbackContext context)
    {
        if (context.performed) player?.Weapon.Kick();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = Helpers.CameraMain.ViewportPointToRay(new Vector3(0.5f, 0f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 3f, LayerMask.GetMask("Hit")))
            {
                hitInfo.collider.TryGetComponent(out IInteractable interactable);
                if (interactable != null)
                {
                    interactable.Interact();
                    Logger.Log($"Interacted with {hitInfo.collider.name}", this, "Interact");
                }
            }

            Debug.DrawRay(ray.origin, ray.direction * 3f, Color.green, 1f);
        }
    }

    public void DirectionalHint(InputAction.CallbackContext context)
    {
        if (!context.performed) return; Camera cam = Camera.main;
        Vector3 spawnPos = cam.transform.position + cam.transform.forward * 2f; 
        spawnPos.y = player.transform.position.y; 
        
        var trail = Instantiate(breadcrumbPrefab, spawnPos, Quaternion.identity); 
        
        trail.Init(breadcrumbTarget, player.transform);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
        }
    }

    void FixedUpdate() //I'm so sorry for my sins!!!!!  // alex: LOL
    {
        if (attackHeld == true)
        {
            switch (player.Weapon.EquippedWeapon)
            {
                case Weapon.Weapons.Staff:
                    player?.Weapon.RangedAttack();
                    break;

                case Weapon.Weapons.Dagger:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
