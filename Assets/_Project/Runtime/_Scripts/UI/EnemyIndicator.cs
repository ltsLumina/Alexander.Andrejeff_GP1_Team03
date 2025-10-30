using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyIndicator : MonoBehaviour
{
    public Image arrow;

    public DetectEnemies detectEnemies;

    List<Image> enemyIndicators;

    void Start()
    {
        
    }

    void Update()
    {
        //initialize indicator for each enemy if not already done
        if (enemyIndicators == null) {
            enemyIndicators = new List<Image>();
        }
        while (enemyIndicators.Count < detectEnemies.enemyAngles.Count)
        {
            Image newIndicator = Instantiate(arrow, arrow.transform.parent);
            enemyIndicators.Add(newIndicator);
        }
        //update each indicator position and rotation based on enemy angle
        for (int i = 0; i < detectEnemies.enemyAngles.Count; i++)
        {
            float angle = detectEnemies.enemyAngles[i].angle;
            PlaceSprite(angle);
            enemyIndicators[i].enabled = true;
        }
        //disable any extra indicators
        for (int i = detectEnemies.enemyAngles.Count; i < enemyIndicators.Count; i++)
        {
            enemyIndicators[i].enabled = false;
        }
    }

    void PlaceSprite(float angle)
    {
        // Get half the Images width and height to adjust it off the screen edge;
        RectTransform arrowRect = arrow.GetComponent<RectTransform>();
        float halfImageWidth = arrowRect.sizeDelta.x / 2f;
        float halfImageHeight = arrowRect.sizeDelta.y / 2f;

        // Get Half the ScreenHeight and Width to position the image
        float halfScreenWidth = (float)Screen.width / 2f;
        float halfScreenHeight = (float)Screen.height / 2f;

        float xPos = 0f;
        float yPos = 0f;

        // Left side of screen;
        if (angle < -45)
        {
            xPos = -halfScreenWidth + halfImageWidth;
            // Ypos can go between +ScreenHeight/2  down to -ScreenHeight/2
            // angle goes between -45 and -135
            // change angle to a value between 0f and 1.0f and Lerp on that
            float normalizedAngle = (angle + 45f) / -90f;
            yPos = Mathf.Lerp(halfScreenHeight, -halfScreenHeight, normalizedAngle);
            // at the top of the screen we need to move the image down half its height
            // at the bottom of the screen we need to move it up half its height
            // in the middle we need to do nothing. so we lerp on the angle again
            float yImageOffset = Mathf.Lerp(-halfImageHeight, halfImageHeight, normalizedAngle);
            yPos += yImageOffset;

        }
        // top of screen
        else if (angle < 45)
        {
            yPos = halfScreenHeight - halfImageHeight;
            float normalizedAngle = (angle + 45f) / 90f;
            xPos = Mathf.Lerp(-halfScreenWidth, halfScreenWidth, normalizedAngle);
            float xImageOffset = Mathf.Lerp(halfImageWidth, -halfImageWidth, normalizedAngle);
            xPos += xImageOffset;
        }
        // right side of screen
        else
        {
            xPos = halfScreenWidth - halfImageWidth;
            float normalizedAngle = (angle - 45) / 90f;
            yPos = Mathf.Lerp(halfScreenHeight, -halfScreenHeight, normalizedAngle);
            float yImageOffset = Mathf.Lerp(-halfImageHeight, halfImageHeight, normalizedAngle);
            yPos += yImageOffset;
        }

        arrowRect.anchoredPosition = new Vector3(xPos, yPos, 0);
        // UI rotation is backwards from our system.  Positive angles go counterclockwise
        arrowRect.Rotate(Vector3.forward, -angle);
    }
}
