using System;
using System.Collections.Generic;
using UnityEngine;

public class SawbladeController : MonoBehaviour
{
    [SerializeField] GameObject MainSaw;
    [SerializeField] GameObject mainMesh;
    [SerializeField] Transform PointA;
    [SerializeField] Transform PointB;
    [SerializeField] float moveTime = 5f;
    [SerializeField] float stayTime = 4f;
    [SerializeField] float turnSpeed = 2f;
    [SerializeField] float damage = 2;
    [Range(0.5f, 3f)]
    [SerializeField] float cooldown = 0.5f;

    private float positionCounter = 0f;
    private float stayCounter = 0f;
    private bool positionFlip = false;
    float timer;
    bool canActivate;
    private List<GameObject> currentTargets = new();
    private ChildCollisionReporter ChildCollider;
    private float convertedStayTime;
    private float convertedMoveTime;
    Vector3 pointAPos;
    Vector3 pointBPos;

    void Start()
    {
        pointAPos = PointA.position;
        pointBPos = PointB.position;

        ChildCollider = GetComponentInChildren<ChildCollisionReporter>();
        ChildCollider.onCollisionEvent += HandleChildCollision;

        convertedStayTime = stayTime * 50f;
        convertedMoveTime = moveTime * 50f;

    }

    void FixedUpdate()
    {
        MainSaw.transform.position = pointAPos * positionCounter / convertedMoveTime + pointBPos * (convertedMoveTime - positionCounter) / convertedMoveTime;

        mainMesh.transform.rotation = mainMesh.transform.rotation * Quaternion.Euler(0f, 0f, turnSpeed);

        if (!positionFlip)
        {
            if (convertedMoveTime <= positionCounter)
            {
                stayCounter -= 1;
                if (stayCounter <= 0)
                {
                    positionFlip = true;
                }
            }
            else
            {
                positionCounter += 1f;
                stayCounter = convertedStayTime;
            }
        }
        else
        {
            if (positionCounter <= 0)
            {
                stayCounter -= 1;
                if (stayCounter <= 0)
                {
                    positionFlip = false;
                }
            }
            else
            {
                positionCounter -= 1f;
                stayCounter = convertedStayTime;
            }
        }
    }

    void Update()
    {
        timer = Mathf.Max(0f, timer - Time.deltaTime);
        canActivate = timer <= 0f;

        if (canActivate && currentTargets.Count > 0)
        {
            foreach (var col in currentTargets)
            {
                if (col == null) continue;

                if (col.TryGetComponent(out IDamageable dmg))
                {
                    dmg.TakeDamage(damage);
                    //Logger.Log($"Sawblade collided! | Tick-Hit: {col.name} | Damage dealt: {damage}", this);
                }
            }

            timer = cooldown;
        }
    }

    void HandleChildCollision(Collider collision, bool entered)
    {
        GameObject other = collision.gameObject;

        if (entered)
        {
            if (!currentTargets.Contains(other))
                currentTargets.Add(other);
        }
        else
        {
            currentTargets.Remove(other);
        }
    }
}
