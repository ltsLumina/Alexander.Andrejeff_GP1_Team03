using System;
using UnityEngine;

[ExecuteInEditMode]
public class ModelSwitcher : MonoBehaviour
{
	[SerializeField] Enemy enemy;

	void Start()
	{
		Enemy.EnemyType type = enemy.Type;

		foreach (Transform child in transform) child.gameObject.SetActive(false);

		Transform mesh = type switch
		{ Enemy.EnemyType.Octopus => transform.GetChild(0),
		  Enemy.EnemyType.Banshee => transform.GetChild(1),
		  Enemy.EnemyType.Debug   => transform.GetChild(0),
		  _                       => throw new ArgumentOutOfRangeException() };

		mesh.gameObject.SetActive(true);
		enemy.Mesh = mesh.gameObject;
	}

	void Update()
	{
		if (Application.isPlaying) return;
		Enemy.EnemyType type = enemy.Type;

		foreach (Transform child in transform) child.gameObject.SetActive(false);

		Transform mesh = type switch
		{ Enemy.EnemyType.Octopus => transform.GetChild(0),
		  Enemy.EnemyType.Banshee => transform.GetChild(1),
		  Enemy.EnemyType.Debug   => transform.GetChild(0),
		  _                       => throw new ArgumentOutOfRangeException() };

		mesh.gameObject.SetActive(true);
	}
}

