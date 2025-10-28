using System;
using UnityEngine;
using VHierarchy.Libs;

public class PlayerHealth : MonoBehaviour
{

    public static Action OnHealthChanged;
    public static Action OnPlayerDied;

    [SerializeField] float maxHealth = 100;
    [SerializeField] float currentHealth = 100;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public bool IsDead => currentHealth <= 0;


    public void TakeDamage(float damage)
    {
        if (currentHealth != 0){
            float newHealth = currentHealth - damage;
            currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
            Debug.Log("Player took: " + damage + ". Current health is: " + currentHealth);
            OnHealthChanged();
        }

        if (IsDead)
        {
            gameObject.Destroy();
            Debug.Log("Player died");
            OnPlayerDied();
            // Do game over screen
        }
    }

    public void IncreaseHealth(float amount)
    {
        float newHealth = currentHealth + amount;
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        OnHealthChanged();

        Debug.Log("Player was healed for: " + amount + ". Current health is: " + currentHealth);
    }

}
