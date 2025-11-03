#region
using System;
using UnityEngine;
using VInspector;
#endregion

public class Chest : MonoBehaviour, IInteractable
{

    public event Action OnRelicChestOpened;
    enum Reward
    {
        Collectable,
        Weapon,
        Heal,
        MaxHealth,
    }

    [Tab("Reward")]
    [SerializeField] Reward reward;
    [SerializeField] GameObject rewardPrefab;
    [EndIf]

    [Tab("Relic")]
    [SerializeField] bool isRelicChest = false;
    [SerializeField] PlayerController playerController;
    [SerializeField] float speedMultiplier;

    LootIndicator lootIndicator;
    bool isOpened = false;
    public bool IsOpened
    {
        get
        {
            return isOpened;
        }
        set
        {
            isOpened = value;
        }
    }

    public InteractableType Type => InteractableType.Chest;

    void Start()
    {
        name = $"Chest ({reward})";

        lootIndicator = GetComponentInChildren<LootIndicator>();
    }

    public void Interact() => Open();

    public void Open()
    {
        if (rewardPrefab != null)
        {
            Instantiate(rewardPrefab, transform.position + Vector3.up, rewardPrefab.transform.rotation);
            gameObject.layer = 0; // non-interactable layer
            isOpened = true;

            if (isRelicChest)
            {
                OnRelicChestOpened?.Invoke();

                if (playerController != null)
                    playerController.BaseMoveSpeed *= speedMultiplier;
            }
        }
    }
}
