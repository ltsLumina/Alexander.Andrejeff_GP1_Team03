using System;
using UnityEngine;
using VInspector;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Weapons/Data", order = 0)]
public class WeaponData : ScriptableObject
{
	[Header("General")]
	[SerializeField] Weapon.Weapons weapon;
	
	[Header("Visuals")]
	[SerializeField] Mesh mesh;
	[SerializeField] Material material;
	
	[Header("Stats")]
	[Range(1, 20)]
	[SerializeField] float damage = 1f;
	[ShowIf(nameof(weapon), Weapon.Weapons.Staff)]
	[SerializeField] GameObject projectilePrefab;
	[EndIf]
	[Range(1, 5f)]
	[SerializeField] float attackRange = 2f;
	[SerializeField] Vector2 attackSize = new  (0.5f, 0.5f);
	[Range(0.01f, 2f)]
	[SerializeField] float attackCooldown = 0.5f;
	
	public float Damage => damage;
	public GameObject ProjectilePrefab => projectilePrefab;
	public float AttackRange => attackRange;
	public Vector2 AttackSize => attackSize;

	public float AttackCooldown => attackCooldown;
	public Mesh Mesh => mesh;
	public Material Material => material;

	public Weapon.Weapons WeaponType => weapon;

	void OnEnable()
	{
		if (string.IsNullOrEmpty(name)) return;
		Debug.Assert(mesh, $"Weapon Data '{name}' is missing a mesh!", this);
	}
}

