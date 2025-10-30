using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WalkToPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    private NavMeshAgent navMeshAgent;
    private float attackDistance = 3.1f;
    private float distanceFromPlayer;
    private IDamageable damageable;

    public int AttackDamage = 10;
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player.TryGetComponent(out damageable);
    }

    void Update()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.position);

        if(distanceFromPlayer > attackDistance)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = (player.position);
            //stop attack anim if any
        }
        else
        {
            navMeshAgent.isStopped = true;
            //attack player
            damageable?.TakeDamage(AttackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
