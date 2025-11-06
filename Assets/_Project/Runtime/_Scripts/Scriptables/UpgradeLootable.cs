using UnityEngine;

public class UpgradeLootable : MonoBehaviour, IInteractable
{
    [SerializeField] Upgrade data;

    public void Interact()
    {
        Logger.Log($"{name} was picked up!", this, "Upgrade");

        var player = FindAnyObjectByType<PlayerController>();
        player.ApplyUpgrade(data);

        Destroy(gameObject);
    }
}
