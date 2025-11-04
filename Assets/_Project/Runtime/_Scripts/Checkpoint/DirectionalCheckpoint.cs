using System;
using UnityEngine;

public class DirectionalCheckpoint : MonoBehaviour
{
    public static event Action<DirectionalCheckpoint> OnDirectionalCheckpointTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnDirectionalCheckpointTriggered?.Invoke(this);
        }
    }
}
