using UnityEngine;

public class RoomMarker : MonoBehaviour
{
    [SerializeField] RoomRegistry room;
    [SerializeField] private Transform checkpoint;

    private void Awake()
    {
        checkpoint = transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var respawn = FindFirstObjectByType<RoomRespawnController>();
        if (respawn) respawn.SetCurrentRoom(room, checkpoint);
    }
}
