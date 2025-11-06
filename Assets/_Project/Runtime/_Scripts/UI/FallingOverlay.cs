using UnityEngine;
using UnityEngine.UI;

public class FallingOverlay : MonoBehaviour
{
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

    void Update() => image.color = player.IsFalling ? Color.Lerp(image.color, Color.white, fadeInSpeed * Time.deltaTime) : Color.Lerp(image.color, Color.clear, fadeOutSpeed * Time.deltaTime);
}
