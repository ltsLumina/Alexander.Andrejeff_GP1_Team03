// ---------- Death -> respawn hook ----------
using System;
using UnityEngine;

public class RoomRespawnController : MonoBehaviour
{
    [SerializeField] RoomRegistry currentRoom;
    [SerializeField] PlayerRespawnable player;
    [SerializeField] Transform checkpoint;
    
    public static event Action<Transform> OnNewCheckpoint;


    // Call this when restarting from checkpoint
    public void OnRespawnButton()
    {
        if (!currentRoom) return;
        currentRoom.RespawnRoom(player, checkpoint);
    }

    public void RespawnPlayer(PlayerRespawnable player, Transform checkpoint)
    {
        if (!currentRoom) return;
        currentRoom.RespawnRoom(player, checkpoint);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            OnRespawnButton();
        }
    }

    // Switch when entering a new room
    public void SetCurrentRoom(RoomRegistry room, Transform m_checkpoint)
    {
        OnNewCheckpoint?.Invoke(m_checkpoint);
        currentRoom = room;
        checkpoint = m_checkpoint;
    }
}
