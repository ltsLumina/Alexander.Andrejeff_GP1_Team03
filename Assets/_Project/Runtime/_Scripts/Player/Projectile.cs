#region
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#endregion

public class Projectile : MonoBehaviour
{
	[SerializeField] WeaponData weaponData;
	[SerializeField] float speed = 10f;
	[SerializeField] float lifetime = 3f;

	[Header("Magnetization")]
	[Tooltip("Distance at which the projectile starts to home in on enemies")]
	[SerializeField] float magnetizationDistance = 5f;
	[Tooltip("How quickly the projectile homes in on enemies")]
	[SerializeField] float magnetizationStrength = 5f;

	[SerializeField] GameObject vfx;
	[SerializeField] GameObject impactVFX;
    [Tooltip("Distance so that enemies that are almost visible are homed in as well")]
    [SerializeField] float onScreenMargin = 0.03f;
	[SerializeField] LayerMask losBlockers;

    [Header("Debug")]
	[SerializeField] bool drawMesh;

    GameObject instance;
    
    public bool Homing { get; set; }
    public bool Piercing { get; set; }

    void Start()
	{
		losBlockers = LayerMask.GetMask("Default", "Ground", "Wall");

		var player = FindAnyObjectByType<PlayerController>();
		Vector3 spawnPos = player.Weapon.StaffTip.transform.position + player.transform.forward;
		instance = Instantiate(vfx, spawnPos, Quaternion.Euler(0f, -90f, 0f), transform);
		instance.transform.localEulerAngles = new (0f, -90f, 0f);

		Destroy(gameObject, lifetime);
	}

	void Update()
	{
		transform.Translate(Vector3.forward * (speed * Time.deltaTime));

		if (!Homing) return;

		List<Enemy> enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();
		enemies.RemoveAll(e => e.CompareTag("Player")); // don't home in on player
		if (enemies.Count == 0) return;

		Enemy closestEnemy = enemies.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).First(e => !e.IsDead);

		var tgt = closestEnemy.transform;
		var col = tgt.GetComponentInChildren<Collider>();
		Vector3 targetPoint = col ? col.bounds.center : tgt.position;

		// use reusable LoS check
		if (!LineOfSightUtility.HasLineOfSightToTarget(tgt, targetPoint, losBlockers, onScreenMargin)) return;

		Vector3 directionToTarget = (closestEnemy.transform.position - transform.position).normalized;
		Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

		if (Vector3.Distance(transform.position, closestEnemy.transform.position) < magnetizationDistance)
		{
			float a = Time.deltaTime * magnetizationStrength;
			float b = (1f - Vector3.Distance(transform.position, closestEnemy.transform.position) / magnetizationDistance);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, a * b);
		}
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (!drawMesh) return;

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, .5f);
	}
#endif

	void OnTriggerEnter(Collider other)
	{
		bool isPlayer = other.CompareTag("Player");
		if (isPlayer) return; // cant shoot urself dummy
		
		if (other.TryGetComponent(out IDamageable damageable))
		{
			damageable.TakeDamage(weaponData.Damage);

			Instantiate(impactVFX, transform.position, Quaternion.identity);
			Destroy(instance?.gameObject);
			Destroy(gameObject);
			return;
		}

		if (!Piercing && other.gameObject.layer == LayerMask.NameToLayer("Wall"))
		{
			Instantiate(impactVFX, transform.position, Quaternion.identity);
			Destroy(instance?.gameObject);
			Destroy(gameObject);
		}
	}
}