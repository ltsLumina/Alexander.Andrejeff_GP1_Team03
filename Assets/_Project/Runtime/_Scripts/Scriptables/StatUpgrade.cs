using UnityEngine;

[CreateAssetMenu(fileName = "Stat Upgrade", menuName = "Upgrades/Stat", order = 1)]
public class StatUpgrade : Upgrade
{
	[SerializeField] protected float moveSpeed;
	[SerializeField] protected float maxHealth;

	public float MoveSpeed => moveSpeed;
	public float MaxHealth => maxHealth;
}
