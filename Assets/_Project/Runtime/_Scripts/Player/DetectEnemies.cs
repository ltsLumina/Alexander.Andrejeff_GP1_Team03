using System.Collections.Generic;
using UnityEngine;


public class DetectEnemies : MonoBehaviour
{
    // detection mutates this only in trigger callbacks
    readonly HashSet<GameObject> enemiesInRange = new();

    // UI reads this; you rebuild it each frame
    public readonly List<(GameObject enemy, float angle)> enemyAngles = new();

    void Update()
    {
        enemyAngles.Clear();
        foreach (var e in enemiesInRange)
            enemyAngles.Add((e, GetAngle(e))); // no structural mutation of the iterated set
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
            Debug.Log("Enemy entered detection range");

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            Debug.Log("Enemy exited detection range");

        }
    }

    float GetAngle(GameObject target)
    {
        // Use Atan2 to avoid your manual quadrant hacks
        var to = target.transform.position - transform.position;
        float angle = Mathf.Atan2(to.x, to.z) * Mathf.Rad2Deg;

        float playerYaw = transform.eulerAngles.y;
        angle -= playerYaw;
        if (angle < -180f) angle += 360f;
        else if (angle > 180f) angle -= 360f;
        return angle;
    }
}
