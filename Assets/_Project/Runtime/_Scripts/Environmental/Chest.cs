using System;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
	[SerializeField] GameObject prefab;
	
	public void Interact() => Open();
	
	public void Open()
	{
		if (prefab != null)
		{
			Instantiate(prefab, transform.position + Vector3.up, prefab.transform.rotation);
		}
	}
}
