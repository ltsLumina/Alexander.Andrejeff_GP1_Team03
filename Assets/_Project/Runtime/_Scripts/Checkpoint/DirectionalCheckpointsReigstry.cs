using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class DirectionalCheckpointsReigstry : MonoBehaviour
{
    public static event Action<Transform> OnNewCheckpointTarget;

    [SerializeField] List<DirectionalCheckpoint> checkpointList;
    Queue<DirectionalCheckpoint> checkpoints;


    private void Awake()
    {
        checkpoints = new Queue<DirectionalCheckpoint>(checkpointList);
    }
    private void OnEnable()
    {
        DirectionalCheckpoint.OnDirectionalCheckpointTriggered += HandleDirectionalCheckpointReached;
    }

    private void OnDisable()
    {
        DirectionalCheckpoint.OnDirectionalCheckpointTriggered -= HandleDirectionalCheckpointReached;
    }
    void HandleDirectionalCheckpointReached(DirectionalCheckpoint dc)
    {
        Debug.Log("Incoming DC: " + dc.name);
        Debug.Log("Peeked DC: " + checkpoints.Peek().name);
       if (checkpoints.Count > 0 && checkpoints.Peek() == dc)
        {
            checkpoints.Dequeue();
            SetNextDirectionalCheckpoint();
        }
     }

    private void SetNextDirectionalCheckpoint()
    {
        DirectionalCheckpoint nextCheckPoint = checkpoints.Peek();
        OnNewCheckpointTarget?.Invoke(nextCheckPoint.transform);
    }

}
