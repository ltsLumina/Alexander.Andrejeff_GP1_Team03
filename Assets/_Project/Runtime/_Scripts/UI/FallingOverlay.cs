using UnityEngine;
using UnityEngine.UI;

public class FallingOverlay : MonoBehaviour
{
    [Tooltip("The Y position at which the overlay starts to fade in. \nBest set to 0.")]
    [SerializeField] float fadeInY;
    [Tooltip("The Y position at which the overlay starts to fade out.")]
    [SerializeField] float fadeOutY = 5f;
    [SerializeField] float fadeInSpeed = 4f;
    [SerializeField] float fadeOutSpeed = 4f;
    
    RawImage image;

    void Awake()
    {
        image = GetComponent<RawImage>();
        image.enabled = true;
        image.color = Color.clear;
    }

    PlayerController player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (player.transform.position.y < fadeInY)
        {
            image.color = Color.Lerp(image.color, Color.white, fadeInSpeed * Time.deltaTime);
        }
        else if (player.transform.position.y < fadeOutY)
        {
            image.color = Color.Lerp(image.color, Color.clear, fadeOutSpeed * Time.deltaTime);
        }
    }
}
