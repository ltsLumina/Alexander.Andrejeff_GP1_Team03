using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthUIHandler : MonoBehaviour
{
    // GameObjects
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] UIDocument UIDoc;

    // UI 
    VisualElement root;

    // UI Containers
    VisualElement healthBarContainer;

    Label healthLabel;
    VisualElement healthBarFill;


    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += HealthChanged;
        PlayerHealth.OnPlayerDied += PlayerDied;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= HealthChanged;
        PlayerHealth.OnPlayerDied -= PlayerDied;
    }

    void Start()
    {
        Debug.Assert(playerHealth != null, "Remember to assign the playerHealth GameObject in the inspector");
        Debug.Assert(UIDoc != null, "Remember to assign the UI Document GameObject in the inspector");
        root = UIDoc.rootVisualElement;

        // Containers
        healthBarContainer = root.Q<VisualElement>("HealthBarContainer");

        healthLabel = root.Q<Label>("HealthLabel");
        healthBarFill = root.Q<VisualElement>("HealthBarFill");

        // Initialize HUD
        healthBarContainer.style.display = DisplayStyle.Flex;
        healthLabel.text = $"{playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";

        
    }

    void HealthChanged()
    {
        float healthRatio = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
        float healthPercent = Mathf.Lerp(0, 100, healthRatio);
        healthBarFill.style.width = Length.Percent(healthPercent);

        healthLabel.text = $"{playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
    }

    void PlayerDied()
    {
        Debug.Log("Cleaning up UI");

        // Turn off player health bar UI
        healthBarContainer.style.display = DisplayStyle.None;        

        // Show Game Over UI
    }

}
