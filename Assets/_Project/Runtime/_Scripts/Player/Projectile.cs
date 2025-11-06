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
		losBlockers = LayerMask.GetMask("Default", "Ground");

        instance = Instantiate(vfx, transform.position, Quaternion.Euler(0f, -90f, 0f), transform);
		instance.transform.localEulerAngles = new (0f, -90f, 0f);

		Destroy(gameObject, lifetime);
	}

	void Update()
	{
		transform.Translate(Vector3.forward * (speed * Time.deltaTime));
		
		if (!Homing) return;

        // look for all nearby IDamageable and home in on the closest one// look for all nearby Enemy and home in on the closest one
        List<Enemy> enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();

		enemies.RemoveAll(e => e.CompareTag("Player")); // dont home in on player
		if (enemies.Count == 0) return;

		// find closest Enemy by distance to its transform
		Enemy closestEnemy = enemies.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).First(e => !e.IsDead);

        var tgt = closestEnemy.transform;
        var col = tgt.GetComponentInChildren<Collider>();
        Vector3 targetPoint = col ? col.bounds.center : tgt.position;

        // on-screen test
        Vector3 eVP = Camera.main.WorldToViewportPoint(targetPoint);
        bool enemyOnScreen =
            eVP.z > 0f &&
            eVP.x > onScreenMargin && eVP.x < 1f - onScreenMargin &&
            eVP.y > onScreenMargin && eVP.y < 1f - onScreenMargin;
        if (!enemyOnScreen) return;

        // line-of-sight from camera
        Vector3 origin = Camera.main.transform.position;
        Vector3 dir = targetPoint - origin;
        float dist = dir.magnitude;
        if (dist <= 0.001f) return; // too close, skip homing this frame

        if (Physics.Raycast(origin, dir / dist, out var hit, dist, losBlockers, QueryTriggerInteraction.Ignore))
        {
            // blocked if we hit anything on losBlockers before reaching the enemy root
            bool hitEnemyRoot = hit.collider.transform.root == tgt.root;
            if (!hitEnemyRoot) return;
        }

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

		if (Piercing) return;
		Destroy(instance?.gameObject);
		Destroy(gameObject);
	}
}
