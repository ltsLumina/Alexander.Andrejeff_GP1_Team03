using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ReticleUIHandler : MonoBehaviour
{
    [SerializeField] UIDocument UIDoc;
    [SerializeField] Texture2D defaultReticle;
    [SerializeField] Texture2D lootableReticle;
    [SerializeField] Texture2D enemyTargetedReticle;

    VisualElement root;
    VisualElement crossHair;

    

    private void OnEnable()
    {
        CrosshairTargetDetector.OnTargetChanged += UpdateCrosshair;
    }

    private void OnDisable()
    {
        CrosshairTargetDetector.OnTargetChanged -= UpdateCrosshair;

    }

    private void Start()
    {
        root = UIDoc.rootVisualElement;

        crossHair = root.Q<VisualElement>("Crosshair");
        crossHair.style.backgroundImage = new StyleBackground(defaultReticle);
    }


    void UpdateCrosshair(IInteractable interactable)
    {
        if (interactable == null)
        {
            crossHair.style.backgroundImage = new StyleBackground(defaultReticle);
            return;
        }

        switch (interactable.InteractableType)
        {
            case InteractableType.Enemy:
                crossHair.style.backgroundImage = new StyleBackground(enemyTargetedReticle);
                break;
        }
    }

}
