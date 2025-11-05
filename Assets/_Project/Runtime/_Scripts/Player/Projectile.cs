#region
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

	[Header("Debug")]
	[SerializeField] bool drawMesh;

	GameObject instance;

	void Start()
	{
		instance = Instantiate(vfx, transform.position, Quaternion.Euler(0f, -90f, 0f), transform);
		instance.transform.localEulerAngles = new (0f, -90f, 0f);

		Destroy(gameObject, lifetime);
	}

	void Update()
	{
		transform.Translate(Vector3.forward * (speed * Time.deltaTime));

		// look for all nearby IDamageable and home in on the closest one// look for all nearby Enemy and home in on the closest one
		List<Enemy> enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();

		enemies.RemoveAll(e => e.CompareTag("Player")); // dont home in on player
		if (enemies.Count == 0) return;

		// find closest Enemy by distance to its transform
		Enemy closestEnemy = enemies.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).First(e => !e.IsDead);

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
		if (!other.TryGetComponent(out IDamageable damageable)) return;
		bool isPlayer = other.CompareTag("Player");
		if (isPlayer) return; // cant shoot urself dummy
		damageable.TakeDamage(weaponData.Damage);

		GameObject impact = Instantiate(impactVFX, transform.position, Quaternion.identity);

		//instance.GetComponentsInChildren<ParticleSystem>().ToList().ForEach(ps => ps.Stop());
		Destroy(instance?.gameObject);
		Destroy(gameObject);
	}
}
