using System;
using DG.Tweening;
using JetBrains.Annotations;
using Lumina.Essentials.Attributes;
using MelenitasDev.SoundsGood;
using UnityEngine;
using VInspector;

public class Weapon : MonoBehaviour
{
	public enum Weapons
	{
		Dagger,
		Staff
	}

	[UsedImplicitly]
	[SerializeField] Weapons equippedWeapon;
	[SerializeField] WeaponData weaponData;

	[Header("Cooldown")]
	[SerializeField, ReadOnly] float attackTime;

	// -- separator
	
	[Tab("Kick")]
	[Header("Kick")]
	[SerializeField] float kickForce = 100f;
	[SerializeField] float verticalMultiplier = 0.15f;
	[SerializeField] Vector2 kickSize = new (0.5f, 0.5f);
	[Range(1, 5f)]
	[SerializeField] float kickRange = 2f;

	[Header("Cooldown"), Range(0.01f, 2f)]
	[SerializeField] float kickCooldown = 0.5f;
	[SerializeField, ReadOnly] float kickTime;

	[Tab("Settings")]
	[SerializeField] MeshFilter meshFilter;
	[SerializeField] Camera fpsCamera;
	
	[Header("Debug")]
	[SerializeField] bool debugDrawRay;
	[SerializeField, ReadOnly] Collider[] hits = new Collider[10];

	Sound kickSFX;
	Sound kickAltSFX;
	
	public Weapons EquippedWeapon
	{
		get => equippedWeapon;
		private set => equippedWeapon = value;
	}

	void OnDrawGizmosSelected()
	{
		Quaternion rot = fpsCamera.transform.rotation;

		Gizmos.color = Color.red;
		(Vector3 attackHalfExtents, Vector3 attackCenter) = GetOverlapBox(false);
		Gizmos.matrix = Matrix4x4.TRS(attackCenter, rot, Vector3.one);
		Gizmos.DrawWireCube(Vector3.zero, attackHalfExtents * 2f);

		Gizmos.color = Color.cyan;
		(Vector3 kickHalfExtents, Vector3 kickCenter) = GetOverlapBox(true);
		Gizmos.matrix = Matrix4x4.TRS(kickCenter, rot, Vector3.one);
		Gizmos.DrawWireCube(Vector3.zero, kickHalfExtents * 2f);
	}

	void Start()
	{
		Equip(weaponData);
		
		#region Sound
		kickSFX = new Sound(SFX.Kick);
		kickSFX.SetVolume(0.3f);
		kickSFX.SetRandomPitch();
		kickSFX.SetSpatialSound();
		kickSFX.SetFollowTarget(transform);	
		
		kickAltSFX = new Sound(SFX.KickAlt);
		kickAltSFX.SetVolume(0.3f);
		kickAltSFX.SetRandomPitch();
		kickAltSFX.SetSpatialSound();
		kickAltSFX.SetFollowTarget(transform);
		#endregion
	}

	void Update()
	{
		attackTime = Mathf.Max(attackTime - Time.deltaTime, 0);
		kickTime = Mathf.Max(kickTime - Time.deltaTime, 0);
	}

	public void Equip(WeaponData data)
	{
		equippedWeapon = data.WeaponType;
		weaponData = data;
		meshFilter.mesh = data.Mesh;
	}

	public void RangedAttack()
	{
		if (attackTime > 0f) return;
		
		Vector3 spawnPos = transform.position;
		var projectile = Instantiate(weaponData.ProjectilePrefab, spawnPos, transform.rotation);

		attackTime = weaponData.AttackCooldown;
	}

	public void Attack()
	{
		if (attackTime > 0f) return;

		if (!GetNearestHitCollider(out Collider[] hitColliders, out Collider nearestHit)) return; // no hits

		if (hitColliders.Length > 1)
		{
			Logger.Log("Multiple hits detected:", this, "Weapon");
			string all = string.Join(", ", Array.ConvertAll(hitColliders, c => c.transform.name));
			Logger.Log("Hits: " + all, this, "Weapon");
		}
		else
		{
			//Logger.Log("Nearest Hit: " + nearestHit.transform.name, this, "Weapon");
		}

		foreach (Collider col in hitColliders)
		{
			if (col.transform.TryGetComponent(out IDamageable damageable))
			{
				damageable.TakeDamage(weaponData.Damage);

				attackTime = weaponData.AttackCooldown;

				//Logger.LogWarning("Dealt " + damage + " damage to " + col.transform.name, this, "Weapon");
			}
		}

		if (Array.TrueForAll(hitColliders, c => !c.transform.TryGetComponent<IDamageable>(out _))) Logger.LogWarning("No damageable component found on any hit colliders. \nThis likely indicates an issue.", this, "Weapon");
	}

	public void Kick()
	{
		if (kickTime > 0f) return;

		if (!GetKickColliders(out Collider[] kickColliders)) return; // no hits

		Enemy enemy = null;
		Crate crate = null;

		foreach (Collider col in kickColliders)
		{
			if (col.TryGetComponent(out Rigidbody rb))
			{
				rb.isKinematic = false;
				var ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
				Vector3 forceDirection = ray.direction.normalized + Vector3.up * verticalMultiplier;
				rb.AddForce(forceDirection * kickForce);

				if (col.TryGetComponent(out enemy)) enemy.Stagger();
				
				if (col.TryGetComponent(out crate)) crate.kickSound.Play(); // ugly but works
				
				kickTime = kickCooldown;
				//Logger.Log("Kicking " + col.transform.name, this, "Weapon");
			}
		}
		
		if (enemy) kickSFX.Play();
		if (crate) kickAltSFX.Play();
	}

	bool GetNearestHitCollider(out Collider[] hitColliders, out Collider nearestHit)
	{
		(Vector3 halfExtents, Vector3 center) = GetOverlapBox(false);
		hits = new Collider[10];
		int length = Physics.OverlapBoxNonAlloc(center, halfExtents, hits, fpsCamera.transform.rotation, LayerMask.GetMask("Hit"));

		if (length <= 0)
		{
			hitColliders = null;
			nearestHit = null;
			return false;
		}

		hitColliders = new Collider[length];
		Array.Copy(hits, hitColliders, length);

		nearestHit = hits[0];
		float nearestDist = Vector3.Distance(fpsCamera.transform.position, nearestHit.transform.position);

		for (int i = 1; i < length; i++)
		{
			float d = Vector3.Distance(fpsCamera.transform.position, hits[i].transform.position);

			if (!(d < nearestDist)) continue;
			nearestDist = d;
			nearestHit = hits[i];
		}

		return hits is { Length: > 0 };
	}

	bool GetKickColliders(out Collider[] kickColliders)
	{
		(Vector3 halfExtents, Vector3 center) = GetOverlapBox(true);

		hits = new Collider[10];
		int length = Physics.OverlapBoxNonAlloc(center, halfExtents, hits, fpsCamera.transform.rotation, LayerMask.GetMask("Hit"));

		if (length <= 0)
		{
			kickColliders = null;
			return false;
		}

		kickColliders = new Collider[length];
		Array.Copy(hits, kickColliders, length);
		return true;
	}

	(Vector3 halfExtents, Vector3 center) GetOverlapBox(bool kick)
	{
		if (fpsCamera == null) return (Vector3.zero, Vector3.zero);

		if (kick)
		{
			Vector3 kickHalfExtents = new Vector3(kickSize.x / 2f, kickSize.y / 2f, kickRange / 2f);
			Vector3 kickFlatCenter = fpsCamera.transform.position + fpsCamera.transform.forward * (kickRange / 2f);

			// Use the camera's Y so the box is centered at the camera height instead of on the ground
			Vector3 kickCenter = new Vector3(kickFlatCenter.x, kickFlatCenter.y, kickFlatCenter.z);
			return (kickHalfExtents, kickCenter);
		}
		else
		{
			Vector2 attackSize = weaponData.AttackSize;
			float attackRange = weaponData.AttackRange;
			Vector3 halfExtents = new Vector3(attackSize.x / 2f, attackSize.y / 2f, attackRange / 2f);
			Vector3 flatCenter = fpsCamera.transform.position + fpsCamera.transform.forward * (attackRange / 2f);

			// Center at camera height
			Vector3 center = new Vector3(flatCenter.x, flatCenter.y, flatCenter.z);
			return (halfExtents, center);
		}
	}

}
