using UnityEngine;

public class turn2DToPlayer : MonoBehaviour
{

    //turn the 2d asset to face the player

    [SerializeField] private Transform player;

    void Update()
    {
        if(player == null)
        {
            return;
        }
        Vector3 targetPos = player.position;
        targetPos.y = transform.position.y; // Keep the y position the same
        transform.LookAt(targetPos);
    }
}
