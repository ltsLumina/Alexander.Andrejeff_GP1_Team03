using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
    LootQueUI lootIndicator;

    private void Start()
    {
        lootIndicator = GetComponentInChildren<LootQueUI>();
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
