using UnityEngine;

[CreateAssetMenu(fileName = "Kick Upgrade", menuName = "Upgrades/Kick", order = 2)]
public class KickUpgrade : Upgrade
{
	[SerializeField] protected float kickForce;
	[SerializeField] protected bool hurtOnWall;
	
	public float KickForce => kickForce;
	public bool HurtOnWall => hurtOnWall;
}
