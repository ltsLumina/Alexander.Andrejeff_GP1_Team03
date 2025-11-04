using System;
using UnityEngine;

public class RotatingBridge : MonoBehaviour, IInteractable
{
    [SerializeField] float targetAngle = 180f;
    [SerializeField] float timeToRotate = 2f;
    [SerializeField] GameObject bridgeObject;

    bool rotate;
    float startY;
    float targetY;
    
    public void Interact()
    {
        if (rotate) return;
        rotate = true;
        startY = bridgeObject.transform.parent.eulerAngles.y;
        targetY = startY + targetAngle;
    }
    
    void Update()
    {
        if (!rotate) return;
    
        Transform parent = bridgeObject.transform.parent;
        float currentY = parent.eulerAngles.y;
        float speed = targetAngle / timeToRotate;
        float newY = Mathf.MoveTowardsAngle(currentY, targetY, speed * Time.deltaTime);
    
        Vector3 euler = parent.eulerAngles;
        euler.y = newY;
        parent.eulerAngles = euler;
    
        if (Mathf.Abs(Mathf.DeltaAngle(newY, targetY)) <= 0.01f)
        {
            rotate = false;
            euler.y = Mathf.Round(targetY / targetAngle) * targetAngle;
            parent.eulerAngles = euler;
        }
    }
}
