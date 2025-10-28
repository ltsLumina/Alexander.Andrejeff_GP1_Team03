using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[SelectionBase, DisallowMultipleComponent, RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody), typeof(Collider))]
public class Enemy : MonoBehaviour, IDamageable
{
    enum EnemyType
    {
        Basic,
        Aerial,
        Debug
    }

    [Header("Enemy")]
    [SerializeField] EnemyType type;
    [SerializeField] float health = 100f;
    [SerializeField] float maxHealth = 100f;

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
        
        Debug.Assert(agent);
    }

    void Start()
    {
        health = maxHealth;

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

    public void TakeDamage(float damage)
    {
        health -= damage;
        transform.DOShakePosition(0.2f, 0.1f, 10, 90, false, true);
        
        Logger.Log("Enemy took: " + damage + " damage.", this, $"{name}");
        
        if (health <= 0) 
            Death();
    }
    
    void Death()
    {
        Logger.LogWarning("Enemy died.", this, $"{name}");
        Destroy(gameObject);
    }
}
