using UnityEngine;
using UnityEngine.AI;

public class BreadcrumbTrail : MonoBehaviour
{
    [SerializeField] GameObject cloudParticlePrefab;
    [SerializeField] float dropInterval = 0.5f;
    [SerializeField] float extraRotationSpeed;

    NavMeshAgent agent;
    float timer = 0f;

    bool isActive;

    public void Init(Transform target, Transform start)
    {

        agent = GetComponent<NavMeshAgent>();
        if (target != null)
        {
            isActive = true;
            agent.SetDestination(target.position);
        }

        Destroy(gameObject, 5);

    }

    void extraRotation()
    {
        Vector3 lookrotation = agent.steeringTarget - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), extraRotationSpeed * Time.deltaTime);

    }

    void Update()
    {
        if (!isActive) return;

        extraRotation();
        if (agent.pathPending || agent.path.corners.Length == 0)
            return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                isActive = false;
                return;
            }
        }

        timer += Time.deltaTime;

        if (timer >= dropInterval)
        {
            SpawnCloud();
            timer = 0f;
        }
    }

    void SpawnCloud()
    {
        if (cloudParticlePrefab == null) return;

        Vector3 pos = transform.position + Vector3.up * 0.1f;

        GameObject cloud = Instantiate(cloudParticlePrefab, pos, Quaternion.identity);

        Destroy(cloud, 5f);


    }
}