using System;
using Lumina.Essentials.Attributes;
using UnityEngine;
using VInspector;

public class Weapon : MonoBehaviour
{
    [Tab("Weapon")]
    [SerializeField] float damage = 10f;
    [SerializeField] float range = 100f;
    [SerializeField] Camera fpsCamera;

    [Tab("Settings")]
    [SerializeField] bool debugDrawRay;
    [SerializeField, ReadOnly] bool blockingHit;

    Collider[] hits;

    public void Attack()
    {
        Vector3 center = fpsCamera.transform.position + fpsCamera.transform.forward * (range * 0.5f);
        Vector3 halfExtents = new Vector3(0.5f, 0.5f, range * 0.5f);
        int length = Physics.OverlapBoxNonAlloc(center, halfExtents, hits, fpsCamera.transform.rotation, LayerMask.GetMask("Hit"));
        
        blockingHit = hits is { Length: > 0 };

        if (blockingHit)
        {
            Collider hitCollider = hits[0];
            float nearestDist = Vector3.Distance(fpsCamera.transform.position, hitCollider.transform.position);

            for (int i = 1; i < hits.Length; i++)
            {
                float d = Vector3.Distance(fpsCamera.transform.position, hits[i].transform.position);

                if (!(d < nearestDist)) continue;
                nearestDist = d;
                hitCollider = hits[i];
            }

            Logger.Log("Hit: " + hitCollider.transform.name, this, "Weapon");

            if (hitCollider.transform.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
                Logger.LogWarning("Dealt " + damage + " damage to " + hitCollider.transform.name, this, "Weapon");
            }
        }
        
        if (debugDrawRay)
        {
            Color rayColor = blockingHit ? Color.green : Color.red;
            Debug.DrawRay(fpsCamera.transform.position, fpsCamera.transform.forward * range, rayColor, 1f);
        }
    }

    public void Kick()
    {
        
    }
}
