using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
    LootIndicator lootIndicator;
    Chest chest;

    private void Start()
    {
        lootIndicator = GetComponentInChildren<LootIndicator>();
        chest = GetComponentInParent<Chest>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !chest.IsOpened) 
            lootIndicator?.ShowIndicator(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            lootIndicator?.ShowIndicator(false);
    }
}
