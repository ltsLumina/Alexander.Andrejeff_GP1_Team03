using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    [SerializeField] GameObject player;
    private PlayerHealth playerHealth;
    [SerializeField] float damage = 13;
    [SerializeField] float heal = 11;
    void Start()
    {
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (playerHealth.IsDead) return;

            playerHealth.TakeDamage(damage);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (playerHealth.IsDead) return;

            playerHealth.IncreaseHealth(heal);
        }
    }
}
