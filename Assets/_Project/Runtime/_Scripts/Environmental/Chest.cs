#region
using System;
using UnityEngine;
using VInspector;
#endregion

public class Chest : MonoBehaviour, IInteractable
{
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

	void Start()
	{
		name = $"Chest ({reward})";
	}

	public void Interact() => Open();
	
	public void Open()
	{
		if (rewardPrefab != null)
		{
			Instantiate(rewardPrefab, transform.position + Vector3.up, rewardPrefab.transform.rotation);
			gameObject.layer = 0; // non-interactable layer
		}
	}
}
