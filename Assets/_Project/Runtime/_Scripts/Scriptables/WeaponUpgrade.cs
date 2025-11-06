using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Upgrade", menuName = "Upgrades/Weapon", order = 0)]
public class WeaponUpgrade : Upgrade
{
	[Tooltip("In percentage.")]
	[SerializeField] protected float cdr = 0.25f;
	[SerializeField] protected bool homing;
	[SerializeField] protected bool piercing;

	public float CDR => cdr;
	public bool Homing => homing;
	public bool Piercing => piercing;
}
