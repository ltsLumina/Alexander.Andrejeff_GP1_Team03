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
    [Tooltip("How quickly the projectile homes in on enemies")]
    [SerializeField] float magnetizationStrength = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        
        // look for all nearby IDamageable and home in on the closest one
        var damageables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IDamageable>()
            .ToList();
        
        damageables.RemoveAll(d => ((Component)d).CompareTag("Player")); // dont home in on player
        if (damageables.Count == 0) return;
        
        // find closest IDamageable by distance to its Component.transform
        IDamageable closest = damageables
            .OrderBy(d => Vector3.Distance(transform.position, ((Component)d).transform.position))
            .First();
        
        Vector3 directionToTarget = (((Component)closest).transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        
        if (Vector3.Distance(transform.position, ((Component)closest).transform.position) < magnetizationDistance)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * magnetizationStrength);
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
