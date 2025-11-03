#region
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VHierarchy.Libs;
#endregion

public class PlayerHealth : MonoBehaviour
{
    public static event Action<float, float> OnHealthChanged; // review: these aren't events yet. Right now its just a delegate, i.e., a function pointer. Make it public static event Action ... to make it an event.
    public static event Action<float, float, bool> OnMaxHealthIncreased;
    public static event Action OnPlayerDied;

    [SerializeField] float maxHealth = 100;
    [SerializeField] float currentHealth = 100;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public bool IsDead => currentHealth <= 0;

    float previousHealth;

    public void IncreaseMaxHealth(float amount, bool refreshCurrentHealth)
    {
        maxHealth += amount;

        if (refreshCurrentHealth) currentHealth = maxHealth;

        OnMaxHealthIncreased?.Invoke(maxHealth, amount, refreshCurrentHealth);

        Debug.Log("Increased player health from: " + (maxHealth - amount) + " to: " + maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth != 0)
        {
            previousHealth = currentHealth;
            float newHealth = currentHealth - damage;
            currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
            /*Logger.Log($"Player took: {damage} damage. " + "\n" +
                       $"Current health is: {currentHealth}", this, "Player");
                       */
            OnHealthChanged?.Invoke(currentHealth, previousHealth);
        }

        if (IsDead)
        {
            GetComponentInChildren<PlayerInput>().actions.Disable();
            Debug.Log("Player died");
            OnPlayerDied?.Invoke();

            // Do game over screen
        }
    }

    public void IncreaseHealth(float amount)
    {
        previousHealth = currentHealth;
        float newHealth = currentHealth + amount;
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, previousHealth);

        Logger.Log("Player was healed for: " + amount + ". Current health is: " + currentHealth, this, "Player");
    }
}
