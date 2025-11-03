using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class GroundHintPath : MonoBehaviour
{
    public Transform player;
    public Transform target;
    public float segmentLength = 1f; //spacing between indicator elements
    public float groundOffset = 0.05f; // lift the elements slightly off the ground
    public LayerMask groundMask;

    private LineRenderer line;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.startWidth = 0.2f;
        line.endWidth = 0.2f;

        Debug.Assert(player != null, "Player reference missing in inspector");
        Debug.Assert(target != null, "Target reference missing in inspector");
    }

    private void Update()
    {
        if (player == null || target == null) return;

        Vector3 start = player.position;
        Vector3 end = target.position;

        float distance = Vector3.Distance(start, end);
        int segments = Mathf.CeilToInt(distance / segmentLength);

        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            Vector3 pos = Vector3.Lerp(start, end, t);

            if (Physics.Raycast(pos + Vector3.up * 5, Vector3.down, out RaycastHit hit, 10f, groundMask))
            {
                points.Add(hit.point + Vector3.up * groundOffset);
            }
            else
            {
                points.Add(pos + Vector3.up * 0.1f);
            }
        }

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }
}
