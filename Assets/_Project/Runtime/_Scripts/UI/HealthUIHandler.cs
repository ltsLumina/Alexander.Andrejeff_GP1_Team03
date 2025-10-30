using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthUIHandler : MonoBehaviour
{
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] UIDocument UIDoc;
    [SerializeField] int heartCount;
    List<VisualElement> hearts = new List<VisualElement>();
    [SerializeField] int healthPerHeart;

    // UI 
    VisualElement root;

    // UI Containers
    VisualElement healthBarContainer;
    VisualElement heartContainer;


    int heartIndex;
    bool lastHeartWasHalf = false;

    [SerializeField] Texture2D heartFull;
    [SerializeField] Texture2D heartHalf;
    [SerializeField] Texture2D heartEmpty;

    private void OnEnable()
    {
        PlayerHealth.OnMaxHealthIncreased += MaxHealthIncreased;
        PlayerHealth.OnHealthChanged += HealthChanged;
        PlayerHealth.OnPlayerDied += PlayerDied;
    }

    private void OnDisable()
    {
        PlayerHealth.OnMaxHealthIncreased -= MaxHealthIncreased;
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
        heartContainer = root.Q<VisualElement>("HeartContainer");


        // Initialize HUD
        CreateHearts();

    }

    void MaxHealthIncreased(float maxHealth, float maxHealthMod, bool refreshCurrentHealth)
    {
        int heartCount = (int)maxHealth / healthPerHeart;

        if (refreshCurrentHealth)
        {
            heartContainer.Clear();
            hearts.Clear();

            for (int i = 0; i < heartCount; i++)
            {
                VisualElement heart = new VisualElement();
                heart.AddToClassList("heart"); // See GameUIStylesheet 
                heart.style.backgroundImage = new StyleBackground(heartFull);
                heartContainer.Add(heart);
                hearts.Add(heart);
            }
            heartIndex = heartCount;
        }
        else
        {
            for (int i = 0; i < maxHealthMod / 2; i++)
            {
                VisualElement heart = new VisualElement();
                heart.AddToClassList("heart"); // See GameUIStylesheet 
                heart.style.backgroundImage = new StyleBackground(heartEmpty);
                heartContainer.Add(heart);
                hearts.Add(heart);
            }
        }
    }

    void CreateHearts()
    {
        heartIndex = heartCount;
        for (int i = 0; i < heartCount; i++)
        {
            VisualElement heart = new VisualElement();
            heart.AddToClassList("heart"); // See GameUIStylesheet 
            heart.style.backgroundImage = new StyleBackground(heartFull);
            heartContainer.Add(heart);
            hearts.Add(heart);
        }
    }


    void AddHeart(float currentHealth, float previousHealth)
    {
        float healAmount = currentHealth - previousHealth;

        for (int i = 0; i < healAmount; i++)
        {

            VisualElement heart = heartContainer.hierarchy.ElementAt(heartIndex - 1);

            if (lastHeartWasHalf)
            {
                heart.style.backgroundImage = new StyleBackground(heartFull);
                lastHeartWasHalf = false;
            }
            else
            {
                heart = heartContainer.hierarchy.ElementAt(heartIndex);
                heart.style.backgroundImage = new StyleBackground(heartHalf);
                lastHeartWasHalf = true;
                heartIndex++;

            }
        }
    }

    void RemoveHeart(float currentHealth, float previousHealth)
    {
        if (heartIndex <= 0) return;

        float damageTaken = previousHealth - currentHealth;

        for (int i = 0; i < damageTaken; i++)
        {
            VisualElement heart = heartContainer.hierarchy.ElementAt(heartIndex - 1);

            if (!lastHeartWasHalf)
            {
                heart.style.backgroundImage = new StyleBackground(heartHalf);
                lastHeartWasHalf = true;
            }
            else
            {
                heart.style.backgroundImage = new StyleBackground(heartEmpty);
                lastHeartWasHalf = false;
                heartIndex--;
            }
        }

    }


    void HealthChanged(float currentHealth, float previousHealth)
    {

        if (currentHealth < previousHealth)
        {
            RemoveHeart(currentHealth, previousHealth);
        }
        else
        {
            AddHeart(currentHealth, previousHealth);
        }
    }

    void PlayerDied()
    {
        Debug.Log("Cleaning up UI");

        // Turn off player health bar UI
        healthBarContainer.style.display = DisplayStyle.None;

        // Show Game Over UI
    }

}
