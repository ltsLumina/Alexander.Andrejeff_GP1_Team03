using System.Linq;
using UnityEngine;

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

	void Start() => Destroy(gameObject, lifetime);

	void Update()
	{
		transform.Translate(Vector3.forward * (speed * Time.deltaTime));

		// look for all nearby IDamageable and home in on the closest one// look for all nearby Enemy and home in on the closest one
		var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();

		enemies.RemoveAll(e => e.CompareTag("Player")); // dont home in on player
		if (enemies.Count == 0) return;

		// find closest Enemy by distance to its transform
		Enemy closestEnemy = enemies.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).First();

		Vector3 directionToTarget = (closestEnemy.transform.position - transform.position).normalized;
		Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

		if (Vector3.Distance(transform.position, closestEnemy.transform.position) < magnetizationDistance) 
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * magnetizationStrength);
	}

	void OnTriggerEnter(Collider other)
	{
		other.TryGetComponent(out IDamageable damageable);
		bool isPlayer = other.CompareTag("Player");
		if (isPlayer) return; // cant shoot urself dummy
		damageable?.TakeDamage(weaponData.Damage);

		Destroy(gameObject, 0.1f);
	}
}
