#region
using System;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

public class PlayerHealth : MonoBehaviour
{
    public static event Action<float, float> OnHealthChanged; // review: these aren't events yet. Right now its just a delegate, i.e., a function pointer. Make it public static event Action ... to make it an event.
    public static event Action<float, float, bool> OnMaxHealthIncreased;
    public static event Action OnPlayerDied;

    [SerializeField] float maxHealth = 100;
    [SerializeField] float currentHealth = 100;
    [SerializeField] RoomRespawnController respawner;
    [SerializeField] PlayerRespawnable player;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public bool IsDead => currentHealth <= 0;
    Transform checkpoint;

    float previousHealth;

    private void OnEnable()
    {
        RoomRespawnController.OnNewCheckpoint += BackToCheckpoint;
    }

    private void OnDisable()
    {
        RoomRespawnController.OnNewCheckpoint -= BackToCheckpoint;
    }

    void BackToCheckpoint(Transform backheckpoint)
    {
        checkpoint = backheckpoint;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) TakeDamage(float.MaxValue);
    }

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
            OnPlayerDied?.Invoke();
            respawner.RespawnPlayer(player, checkpoint);
            currentHealth = maxHealth;
            Logger.LogWarning("Player died");
            OnMaxHealthIncreased?.Invoke(maxHealth, maxHealth, true);
            //IncreaseHealth(currentHealth);

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
