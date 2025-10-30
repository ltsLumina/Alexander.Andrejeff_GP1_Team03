using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


public class DetectEnemies : MonoBehaviour
{

    //detect enemies in range and give it to UI if they are not on view to show on screen edges as red dots or something


    //public List<Transform> detectedEnemies;

    public List<(GameObject enemy, float angle)> enemyAngles = new();

    void Update()
    {
        //calculate angles to all detected enemies
        foreach (var enemyAngle in enemyAngles)
        {
            float angle = GetAngle(enemyAngle.enemy);
            //update angle in list
            int index = enemyAngles.FindIndex(e => e.enemy == enemyAngle.enemy);
            if (index != -1)
            {
                enemyAngles[index] = (enemyAngle.enemy, angle);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(CompareTag("Hit"))
        {
            //add enemy to list
            float angle = GetAngle(other.gameObject);
            enemyAngles.Add((other.gameObject, angle));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CompareTag("Hit"))
        {
            //remove enemy from list
            enemyAngles.RemoveAll(e => e.enemy == other.gameObject);
        }
    }

    float GetAngle(GameObject target)
    {
        float angle;
        float xDiff = target.transform.position.x - transform.position.x;
        float zDiff = target.transform.position.z - transform.position.z;

        angle = Mathf.Atan(xDiff / zDiff) * 180f / Mathf.PI;
        // tangent only returns an angle from -90 to +90.  we need to check if its behind us and adjust.
        if (zDiff < 0)
        {
            if (xDiff >= 0)
                angle += 180f;
            else
                angle -= 180f;
        }

        // this is our angle of rotation from 0->360
        float playerAngle = transform.eulerAngles.y;
        // we  need to adjust this to our -180->180 system.
        if (playerAngle > 180f)
            playerAngle = 360f - playerAngle;

        /*
         * this might be bugfix
         * if (playerAngle > 180f)
            playerAngle = playerAngle - 360f;
         */

        // now subtract the player angle to get our relative angle to target.
        angle -= playerAngle;

        // Make sure we didn't rotate past 180 in either direction
        if (angle < -180f)
            angle += 360;
        else if (angle > 180f)
            angle -= 360;

        // now we have our correct relative angle to the target between -180 and 180
        // Lets clamp it between -135 and 135
      //  Mathf.Clamp(angle, -135f, 135f);
        return angle;
    }
}
