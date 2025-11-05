using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIndicator : MonoBehaviour
{
    [Header("References")]
    public Image arrow;                   // Disabled template in Canvas
    public DetectEnemies detectEnemies;   // Drag player�s DetectEnemies here

    [Header("Padding")]
    [SerializeField] float edgePadding = 24f;
    [SerializeField] float onScreenMargin = 0.03f;     // hide when near-visible
    [SerializeField] float spriteForwardOffset = 180f;
    [Tooltip("assign cam on player")]
    [SerializeField] Camera cam;                       // assign Main Camera

    [Header("SizeNColor")]
    [SerializeField] float minDist = 2f;      // start of full size/opacity
    [SerializeField] float maxDist = 40f;     // farthest distance considered
    [SerializeField] AnimationCurve scaleByT = AnimationCurve.Linear(0, 1f, 1, 0.6f);
    [SerializeField] AnimationCurve alphaByT = AnimationCurve.Linear(0, 1f, 1, 0.35f);

    readonly List<Image> enemyIndicators = new();
    Canvas canvas;
    RectTransform container;                             // IndicatorsRoot

    readonly List<(GameObject enemy, float angle)> buffer = new();

    void Awake()
    {
        container = (RectTransform)transform;            // this script on IndicatorsRoot
        canvas = GetComponentInParent<Canvas>();
    }

    void LateUpdate()
    {
        var enemies = detectEnemies.enemyAngles;

        buffer.Clear();
        var src = detectEnemies.enemyAngles;
        for (int i = 0; i < src.Count; i++)
        {
            var e = src[i].enemy;
            if (e) buffer.Add(src[i]); // skip destroyed
        }

        // Spawn more indicators if needed
        while (enemyIndicators.Count < buffer.Count)
        {
            Image inst = Instantiate(arrow, arrow.transform.parent);
            inst.gameObject.SetActive(true);           // ensure clone is active
            enemyIndicators.Add(inst);
        }

        // Update indicators
        for (int i = 0; i < buffer.Count; i++)
        {
            if (enemies[i].enemy == null)
            {
                enemyIndicators[i].enabled = false;
                continue;
            }
            
            float angle = enemies[i].angle;
            var rect = enemyIndicators[i].rectTransform;



            var w = enemies[i].enemy.transform.position;
            var v = cam.WorldToViewportPoint(w);

            bool inFront = v.z > 0f;
            bool onScreen = inFront &&
                            v.x > onScreenMargin && v.x < 1f - onScreenMargin &&
                            v.y > onScreenMargin && v.y < 1f - onScreenMargin;

            if (onScreen)
            {
                enemyIndicators[i].enabled = false;
                continue;
            }

            float dist = Vector3.Distance(detectEnemies.transform.position,
                              enemies[i].enemy.transform.position);
            float t = Mathf.InverseLerp(minDist, maxDist, dist);
            float s = Mathf.Clamp(scaleByT.Evaluate(t), 0.2f, 2f);
            float a = Mathf.Clamp01(alphaByT.Evaluate(t));

            // scale
            rect.localScale = Vector3.one * s;

            // alpha
            var img = enemyIndicators[i];
            var c = img.color; c.a = a; img.color = c;

            PlaceSpriteFor(rect, angle);
            enemyIndicators[i].enabled = true;
        }

        // Hide extras
        for (int i = buffer.Count; i < enemyIndicators.Count; i++)
            enemyIndicators[i].enabled = false;
    }



    void PlaceSpriteFor(RectTransform rect, float angleDeg)
    {
        float minX = edgePadding;
        float maxX = Screen.width - edgePadding;
        float minY = edgePadding;
        float midY = Screen.height * 0.5f - edgePadding; // cap at half-screen

        Vector2 center = new(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 dir = AngleToDir2D(angleDeg);

        // Intersect only with left/right/bottom of the bottom-half rect
        Vector2 hit = RayToBottomHalfRect(center, dir, minX, maxX, minY, midY);

        // Screen -> local UI
        var camForRT = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera ?? cam;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(container, hit, camForRT, out var local);

        rect.anchoredPosition = local;
        rect.localRotation = Quaternion.Euler(0f, 0f, -(angleDeg + spriteForwardOffset));
    }

    static Vector2 AngleToDir2D(float deg)
    {
        float r = deg * Mathf.Deg2Rad;
        // +x right, +y up (screen coords)
        return new Vector2(Mathf.Sin(r), Mathf.Cos(r)).normalized;
    }

    // Robust: prefers Left/Right, never the top half, no "middle" snap.
    static Vector2 RayToBottomHalfRect(Vector2 c, Vector2 d, float minX, float maxX, float minY, float midY)
    {
        const float PIX = 1.0f; // pixel tolerance
        float tBest = float.PositiveInfinity;
        Vector2 pBest = c;

        // Try RIGHT
        if (Mathf.Abs(d.x) > 1e-6f)
        {
            float tR = (maxX - c.x) / d.x;
            if (tR > 0f)
            {
                float y = c.y + d.y * tR;
                if (y >= minY - PIX && y <= midY + PIX && tR < tBest)
                {
                    tBest = tR;
                    pBest = new Vector2(maxX, Mathf.Clamp(y, minY, midY));
                }
            }

            // Try LEFT
            float tL = (minX - c.x) / d.x;
            if (tL > 0f)
            {
                float y = c.y + d.y * tL;
                if (y >= minY - PIX && y <= midY + PIX && tL < tBest)
                {
                    tBest = tL;
                    pBest = new Vector2(minX, Mathf.Clamp(y, minY, midY));
                }
            }
        }

        // Try BOTTOM
        if (Mathf.Abs(d.y) > 1e-6f)
        {
            float tB = (minY - c.y) / d.y; // bottom edge at y=minY
            if (tB > 0f)
            {
                float x = c.x + d.x * tB;
                if (x >= minX - PIX && x <= maxX + PIX && tB < tBest)
                {
                    tBest = tB;
                    pBest = new Vector2(Mathf.Clamp(x, minX, maxX), minY);
                }
            }
        }
        // Fallback: ray aimed into top half, pick side by horizontal direction
        if (float.IsPositiveInfinity(tBest))
        {
            bool goRight = d.x > 0f;
            return new Vector2(goRight ? maxX : minX, midY);
        }

        return pBest;
    }
}