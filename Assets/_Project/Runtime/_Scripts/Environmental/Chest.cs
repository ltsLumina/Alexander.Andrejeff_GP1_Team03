#region
using System;
using MelenitasDev.SoundsGood;
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
    Sound tonalHint;

    public bool IsOpened { get; private set; }

    public InteractableType Type => InteractableType.Chest;

    void Start()
    {
        name = $"Chest ({reward})";

        lootIndicator = GetComponentInChildren<LootIndicator>();
        
        tonalHint = new Sound(SFX.TonalHint);
        tonalHint.SetOutput(Output.SFX);
        tonalHint.SetSpatialSound();
        tonalHint.SetVolume(0.3f);
        tonalHint.SetPosition(transform.position);
    }

    public void Interact() => Open();

    public void Open()
    {
        if (rewardPrefab != null)
        {
            Instantiate(rewardPrefab, transform.position + Vector3.up, rewardPrefab.transform.rotation);
            tonalHint.Play();
            gameObject.layer = 0; // non-interactable layer
            IsOpened = true;

            if (isRelicChest)
            {
                OnRelicChestOpened?.Invoke();

                if (playerController != null)
                    playerController.BaseMoveSpeed *= speedMultiplier;
            }
        }
    }
}
