using System;
using System.Collections.Generic;
using UnityEngine;

public class SawbladeController : MonoBehaviour
{
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
    IDamageable target;

    void FixedUpdate()
    {
        transform.position = PointA.position * positionCounter / moveTime + PointB.position * (moveTime - positionCounter) / moveTime;

        if (positionFlip == false)
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
            if (0 >= positionCounter)
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
                    Logger.Log($"Sawblade collided! | Tick-Hit: {col.name} | Damage dealt: {damage}");
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
