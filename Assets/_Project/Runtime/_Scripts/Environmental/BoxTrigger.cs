#region
using UnityEngine;
#endregion

public class BoxTrigger : MonoBehaviour
{
	LootIndicator lootIndicator;
	Chest chest;

	void Start()
	{
		lootIndicator = GetComponentInChildren<LootIndicator>();
		chest = GetComponentInParent<Chest>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !chest.IsOpened) lootIndicator?.ShowIndicator(true);
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player")) lootIndicator?.ShowIndicator(false);
	}
}
