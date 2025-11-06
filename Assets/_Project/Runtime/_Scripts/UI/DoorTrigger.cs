using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    LootIndicator lootIndicator;

    private void Start()
    {
        lootIndicator = GetComponentInChildren<LootIndicator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            lootIndicator?.ShowIndicator(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            lootIndicator?.ShowIndicator(false);
    }
}
