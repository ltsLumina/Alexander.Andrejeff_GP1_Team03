// ---------- Contracts ----------
using System.Collections.Generic;
using UnityEngine;

//if we want to reset other room elements (traps, puzzles, etc.) on player respawn, we can use this interface

/*
public interface IRoomResettable
{
    // Called on player respawn inside the same room.
    void OnRoomReset();
}
*/

public interface IEnemyReset
{
    void ResetToStart();
}


public class RoomRegistry : MonoBehaviour
{
    public List<IEnemyReset> enemies = new();

    public void Register(IEnemyReset e) { if (!enemies.Contains(e)) enemies.Add(e); }
    public void Unregister(IEnemyReset e) { enemies.Remove(e); }

    public void RespawnRoom(PlayerRespawnable player, Transform checkpoint)
    {
        player.TeleportTo(checkpoint.position, checkpoint.rotation);

        // Compact the list to purge any stray nulls from destroyed objects.
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] == null) enemies.RemoveAt(i);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].ResetToStart();
        }

    }
}
