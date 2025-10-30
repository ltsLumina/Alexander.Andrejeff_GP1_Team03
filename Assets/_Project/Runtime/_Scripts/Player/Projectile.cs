using System;
using System.Linq;
using UnityEngine;
using VInspector.Libs;

public class Projectile : MonoBehaviour
{
    [SerializeField] WeaponData weaponData; 
    [SerializeField] float speed = 10f;
    [SerializeField] float lifetime = 3f;
    
    [Header("Magnetization")]
    [Tooltip("Distance at which the projectile starts to home in on enemies")]
    [SerializeField] float magnetizationDistance = 5f;
    [SerializeField] float magnetizationSpeed = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        
        // look for all nearby enemies and home in on the closest one
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        enemies.ToList().SortBy(e => Vector3.Distance(transform.position, e.transform.position));
        if (enemies.Length == 0) return;
        
        var closestEnemy = enemies[0];
        Vector3 directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);

        if (Vector3.Distance(transform.position, closestEnemy.transform.position) < magnetizationDistance)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * magnetizationSpeed);
        }
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
