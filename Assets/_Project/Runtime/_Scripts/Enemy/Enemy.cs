using System;
using System.Collections;
using DG.Tweening;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.AI;
using VInspector;
using Random = UnityEngine.Random;

[SelectionBase, DisallowMultipleComponent, RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody), typeof(CapsuleCollider))]
public class Enemy : MonoBehaviour, IDamageable
{
    enum EnemyType
    {
        Basic,
        Aerial,
        Debug
    }

    [Tab("Enemy")]
    [SerializeField] EnemyType type;
    [Range(0,20)]
    [SerializeField] float health = 20f;
    [Range(1,20)]
    [SerializeField] float maxHealth = 20f;
    [SerializeField, ReadOnly] float hearts = 10f;

    [Tab("NavMesh")]
    [SerializeField] Transform target;

    public float Health => health;
    public float MaxHealth => maxHealth;

    NavMeshAgent agent;
    Rigidbody rb;
    Collider col;
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        
        Debug.Assert(agent, "NavMeshAgent component is missing!", this);
        Debug.Assert(target, "Target field is not assigned! \nThis should almost always be the player.", this);
    }

    void Start()
    {
        health = maxHealth;
        hearts = health / 2f;

        #region could do this with types:
        switch (type)
        {
            case EnemyType.Basic:
                // nothing special
                break;

            case EnemyType.Aerial:
                //transform.localScale *= 0.8f;
                //health *= 0.8f;
                break;

            case EnemyType.Debug:
                //transform.localScale *= 2f;
                //health *= 2f;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
        #endregion
        
        name = $"Enemy | {type.ToString()} | ({Random.Range(1000, 9999)})";
    }

    void Update()
    {
        hearts = health / 2f;
        
        float offsetDistance = 2f;
        Vector3 desired = target.position + target.forward * offsetDistance;

        // returns the nearest point on the navmesh to the desired position
        Vector3 nearestPoint = NavMesh.SamplePosition(desired, out NavMeshHit hit, 1.0f, NavMesh.AllAreas) ? hit.position : desired;
        agent.SetDestination(nearestPoint);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        transform.DOShakePosition(0.2f, 0.1f, 10, 90, false, true).SetLink(gameObject);
        
        Logger.Log("Enemy took: " + damage + " damage.", this, $"{name}");
        
        if (health <= 0) 
            Death();
    }
    
    void Death()
    {
        Logger.LogWarning("Enemy died.", this, $"{name}");
        Destroy(gameObject);
    }
    
    public void Stagger(float duration)
    {
        StartCoroutine(PerformStagger(duration));
    }
    
    IEnumerator PerformStagger(float duration)
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(duration);
        agent.isStopped = false;

        rb.isKinematic = true;
    }
}
