using System;
using System.Collections.Generic;
using UnityEngine;

public class SawbladeController : MonoBehaviour
{
    [SerializeField] GameObject MainSaw;
    [SerializeField] Transform PointA;
    [SerializeField] Transform PointB;
    [SerializeField] float moveTime = 250f;
    [SerializeField] float stayTime = 100f;
    [SerializeField] float damage = 2;
    [Range(0.5f, 3f)]
    [SerializeField] float cooldown = 0.5f;

    private float positionCounter = 0f;
    private float stayCounter = 0f;
    private bool positionFlip = false;
    float timer;
    bool canActivate;
    private List<Collider> currentTargets = new();
    Vector3 pointAPos;
    Vector3 pointBPos;

    void Start()
    {
        pointAPos = PointA.position;
        pointBPos = PointB.position;
    }

    void FixedUpdate()
    {
        MainSaw.transform.position = pointAPos * positionCounter / moveTime + pointBPos * (moveTime - positionCounter) / moveTime;

        if (!positionFlip)
        {
            if (moveTime <= positionCounter)
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
                stayCounter = stayTime;
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
                stayCounter = stayTime;
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

    void OnTriggerEnter(Collider other)
    {
        if (!currentTargets.Contains(other))
        {
            currentTargets.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        currentTargets.Remove(other);
    }
}
