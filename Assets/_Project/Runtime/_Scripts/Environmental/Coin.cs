using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Transform mesh;
    [SerializeField] int coinValue = 1;
    [SerializeField] float turnSpeed = 2f;
    [SerializeField] float collectionTurnSpeed = 6f;
    [SerializeField] float coinActivationDistance = 5f;
    [SerializeField] float coinGlideTime = 40f;
    [SerializeField] float coinBounceHeight = 1f;
    private float coinRotate;
    private float currentCoinGlideTime;
    private bool doDaThing = false; // Brainblock, this was the most comprehensive name I could give the variable
    Vector3 bounceHeight;
    Vector3 oldPosition;
    SphereCollider myCollider;

    void Awake()
    {
        myCollider = GetComponent<SphereCollider>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            Debug.Log("Coin is using a backup method of getting player, please asign in field.");
        }

        oldPosition = transform.position;
        bounceHeight.y = coinBounceHeight;

        myCollider.radius = coinActivationDistance;
        currentCoinGlideTime = coinGlideTime;
    }

    void FixedUpdate()
    {
        coinRotate += turnSpeed;

        if (doDaThing)
        {
            bounceHeight.y = coinBounceHeight * (1 - Mathf.Abs(currentCoinGlideTime - coinGlideTime / 2) / (coinGlideTime / 2));

            transform.position =
                oldPosition * (currentCoinGlideTime / coinGlideTime) +
                player.transform.position * ((coinGlideTime - currentCoinGlideTime) / coinGlideTime) +
                bounceHeight;
            currentCoinGlideTime -= 1f;

            if (currentCoinGlideTime <= 1)
            {
                GameManager.Instance.playerCoins += coinValue;
                Destroy(gameObject);
            }
            coinRotate += collectionTurnSpeed;
        }

        mesh.rotation = Quaternion.Euler(0f, coinRotate, 0f);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doDaThing = true;
        }
    }
}
