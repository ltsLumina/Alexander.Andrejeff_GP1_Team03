using System;
using Lumina.Essentials.Attributes;
using UnityEngine;
using VInspector;

public class Weapon : MonoBehaviour
{
    [Tab("Config")]
    [Header("Weapon")]
    [SerializeField] Camera fpsCamera;
    [SerializeField] float damage = 2f;
    [SerializeField] Vector2 attackSize = new (0.5f, 0.5f);
    [Range(1, 5f)]
    [SerializeField] float attackRange = 2f;
    
    [Header("Cooldown"), Range(0.01f, 2f)]
    [SerializeField] float attackCooldown = 0.5f;
    [SerializeField, ReadOnly] float attackTime;
    
    [Header("Kick")]
    [SerializeField] float kickForce = 100f;
    [SerializeField] Vector2 kickSize = new (0.5f, 0.5f);
    [Range(1, 5f)]
    [SerializeField] float kickRange = 2f;
    
    [Header("Cooldown"), Range(0.01f, 2f)]
    [SerializeField] float kickCooldown = 0.5f;
    [SerializeField, ReadOnly] float kickTime; 

    [Tab("Settings")]
    [SerializeField] bool debugDrawRay;
    [SerializeField, ReadOnly] bool blockingHit;
    [SerializeField, ReadOnly] Collider[] hits = new Collider[10];

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

    void Update()
    {
        attackTime = Mathf.Max(attackTime - Time.deltaTime, 0);
        kickTime = Mathf.Max(kickTime - Time.deltaTime, 0);
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
            Logger.Log("Nearest Hit: " + nearestHit.transform.name, this, "Weapon");
        }

        foreach (Collider col in hitColliders)
        {
            if (col.transform.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
                
                attackTime = attackCooldown;
                Logger.LogWarning("Dealt " + damage + " damage to " + col.transform.name, this, "Weapon");
            }
        }
        
        if (Array.TrueForAll(hitColliders, c => !c.transform.TryGetComponent<IDamageable>(out _))) 
            Logger.LogWarning("No damageable component found on any hit colliders. \nThis likely indicates an issue.", this, "Weapon");
    }

    public void Kick()
    {
        if (kickTime > 0f) return;
        
        if (!GetKickColliders(out Collider[] kickColliders)) return; // no hits

        foreach (Collider col in kickColliders)
        {
            if (col.TryGetComponent(out Rigidbody rb))
            {
                Vector3 forceDirection = (col.transform.position - fpsCamera.transform.position).normalized; // away from player
                rb.isKinematic = false;
                rb.AddForce(forceDirection * kickForce);

                if (col.TryGetComponent(out Enemy enemy)) 
                    enemy.Stagger(1.5f);

                kickTime = kickCooldown;
                Logger.Log("Kicking " + col.transform.name, this, "Weapon");
            }
        }
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
            return blockingHit = false;
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
        
        return blockingHit = hits is { Length: > 0 };
    }
    
    bool GetKickColliders(out Collider[] kickColliders)
    {
        
        Vector3 halfExtents = new Vector3(kickSize.x / 2f, kickSize.y / 2f, kickRange / 2f);

        // horizontal center in front of the camera
        Vector3 flatCenter = fpsCamera.transform.position + fpsCamera.transform.forward * (kickRange / 2f);

        // place box so its bottom rests on the ground (y = 0)
        Vector3 center = new Vector3(flatCenter.x, halfExtents.y, flatCenter.z);
        
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
        if (kick)
        {
            Vector3 kickHalfExtents = new Vector3(kickSize.x / 2f, kickSize.y / 2f, kickRange / 2f);

            // horizontal center in front of the camera
            Vector3 kickFlatCenter = fpsCamera.transform.position + fpsCamera.transform.forward * (kickRange / 2f);

            // place box so its bottom rests on the ground (y = 0)
            Vector3 kickCenter = new Vector3(kickFlatCenter.x, kickHalfExtents.y, kickFlatCenter.z);
            return (kickHalfExtents, kickCenter);
        }
        else
        {
            Vector3 halfExtents = new Vector3(attackSize.x / 2f, attackSize.y / 2f, attackRange / 2f);

            // horizontal center in front of the camera
            Vector3 flatCenter = fpsCamera.transform.position + fpsCamera.transform.forward * (attackRange / 2f);

            // place box so its bottom rests on the ground (y = 0)
            Vector3 center = new Vector3(flatCenter.x, halfExtents.y, flatCenter.z);
            return (halfExtents, center);
        }
    }
}
