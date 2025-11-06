#region
using System;
using MelenitasDev.SoundsGood;
using UnityEngine;
using VInspector;
#endregion

public class Chest : MonoBehaviour, IInteractable
{
    enum Reward
    {
        Upgrade,
        Weapon,
        Relic,
        Other,
    }

    [Tab("Reward")]
    [SerializeField] Reward reward;
    [SerializeField] GameObject rewardPrefab;
    [SerializeField] float offset = 0.75f;
    [SerializeField] float scaleFactor = 1.5f;
    [EndIf]

    LootIndicator lootIndicator;
    Sound tonalHint;

    public bool IsOpened { get; private set; }

    public InteractableType Type => InteractableType.Chest;

    public event Action OnRelicChestOpened; 

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
            var obj = Instantiate(rewardPrefab, transform.position + Vector3.up * offset, transform.rotation);
            obj.transform.localScale *= scaleFactor;
            tonalHint.Play();
            gameObject.layer = 0; // non-interactable layer
            IsOpened = true;
        }

        if (reward == Reward.Relic) OnRelicChestOpened?.Invoke();
    }
}
