using Lumina.Essentials.Modules;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairTargetDetector : MonoBehaviour
{
    public static event Action<IInteractable> OnTargetChanged;

    [SerializeField] float rayDistance = 10f;

    GameObject lastHitObject;

    void Update()
    {
        UpdateCrosshair();
    }

    void UpdateCrosshair()
    {
        Ray ray = Helpers.CameraMain.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);
        IInteractable interactable = default;

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {

            if (hit.collider.TryGetComponent(out interactable))
            {
                if (hit.collider.gameObject == lastHitObject) return;

                lastHitObject = hit.collider.gameObject;

                if (interactable.InteractableType == InteractableType.Enemy)
                {
                    Debug.Log(InteractableType.Enemy + " hit");
                    OnTargetChanged?.Invoke(interactable);
                }
            }
            else
            {
                if (lastHitObject != null)
                {
                    lastHitObject = null;
                    OnTargetChanged?.Invoke(interactable);
                    Debug.Log("No interactable target hit");
                }
            }
        }
        else
        {
            if (lastHitObject != null)
            {
                lastHitObject = null;
                OnTargetChanged?.Invoke(interactable);
                Debug.Log("No interactable target hit");
            }
        }

    }
}
