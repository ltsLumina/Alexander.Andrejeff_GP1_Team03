using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] Transform parent;
    [SerializeField] InputManager inputs;
    [SerializeField] Vector3 defaultRelativeCameraPosition = new Vector3(0, 0.585f, -0.125f);
    [SerializeField] float cameraBobIntensity = 0.05f;
    [SerializeField] float cameraBobSpeed = 1000f;
    [SerializeField] float cameraIdleBob = 0.2f;
    [SerializeField] public bool bobbingEnabled = true;
    [SerializeField] float shakeIntensity = 1f;
    [SerializeField] public bool viewShakeEnabled = true;

    Vector3 bobbingPosition = new Vector3(0, 0, 0);
    Vector3 shakePosition = new Vector3(0, 0, 0);
    float viewBobbingMult;
    float veiwBobbingCounter;
    float viewSmoothInOut = 1f;
    float viewShakeX;
    float viewShakeY;
    readonly float baseShake = 0.05f;

    void Start()
    {
        PlayerHealth.OnHealthChanged += DoCameraShake;
    }

    void OnDestroy()
    {
        PlayerHealth.OnHealthChanged -= DoCameraShake;
    }
    void FixedUpdate()
    {
        if (bobbingEnabled == true)
        {
            if (viewBobbingMult == 0f)
            {
                if (viewSmoothInOut > cameraIdleBob)
                {
                    viewSmoothInOut -= 0.02f;
                }
            }
            else
            {
                if (viewSmoothInOut < 1f)
                {
                    viewSmoothInOut += 0.02f;
                }
            }
        }
        else
        {
            viewSmoothInOut = 0f;
        }
    }

    void Update()
    {
        // Bobbing logic
        viewBobbingMult = inputs.MoveInput.y;

        veiwBobbingCounter += viewSmoothInOut * cameraBobSpeed * Time.deltaTime;

        bobbingPosition.y = Mathf.Cos(veiwBobbingCounter * Mathf.Deg2Rad) * cameraBobIntensity * viewSmoothInOut;
        bobbingPosition.x = Mathf.Sin(veiwBobbingCounter / 2 * Mathf.Deg2Rad) * cameraBobIntensity * viewSmoothInOut;

        // Shake logic
        if (Mathf.Round(shakePosition.x * 100) == 0f && Mathf.Round(viewShakeX * 100) == 0f)
        {
            shakePosition.x = 0f;
        }
        else
        {
            shakePosition.x = shakePosition.x / 1.06f + viewShakeX;
            viewShakeX = viewShakeX / 1.1f;
        }

        if (Mathf.Round(shakePosition.y * 100) == 0f && Mathf.Round(viewShakeY * 100) == 0f)
        {
            shakePosition.y = 0f;
        }
        else
        {
            shakePosition.y = shakePosition.y / 1.06f + viewShakeY;
            viewShakeY = viewShakeY / 1.1f;
        }

        // Position setter
        transform.localPosition = defaultRelativeCameraPosition + bobbingPosition + shakePosition;
    }

    private void DoCameraShake(float currentHealth, float previousHealth)
    {
        if (currentHealth - previousHealth < 0 && viewShakeEnabled == true)
        {
            viewShakeX = (Random.value - 0.5f) * shakeIntensity * baseShake;
            viewShakeY = (Random.value - 0.5f) * shakeIntensity * baseShake;
        }
    }
}
