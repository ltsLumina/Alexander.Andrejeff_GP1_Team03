using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using VInspector;

public class CameraScript : MonoBehaviour
{
    [Tab("General")]
    [SerializeField] Transform parent;
    [SerializeField] InputManager inputs;
    [SerializeField] Vector3 defaultRelativeCameraPosition = new Vector3(0, 0.585f, -0.125f);

    [Tab("Camera Bobbing")]
    [SerializeField] bool bobbingEnabled = true;
    [ShowIf(nameof(BobbingEnabled), true)]
    [SerializeField] float cameraBobIntensity = 0.05f;
    [SerializeField] float cameraBobSpeed = 1000f;
    [SerializeField] float cameraIdleBob = 0.2f;
    [EndIf]

    [Tab("Camera Shake")]
    [SerializeField] bool viewShakeEnabled = true;
    [ShowIf(nameof(ViewShakeEnabled), true)]
    [SerializeField] float shakeIntensity = 1f;
    [EndIf]

    [Tab("Aim Assist")]
    [SerializeField] bool aimAssistEnabled = true;
    [ShowIf(nameof(aimAssistEnabled), true)]
    [SerializeField] float aimAssistAngleThreshold = 30f;
    [SerializeField] float aimAssistMinDistance = 10f;
    // Aim assist camera reset
    [SerializeField] float aimAssistTimer = 1.5f;
    float aimAssistReset = 1.5f;
    Quaternion defaultRotation;
    [EndIf]

    Vector3 bobbingPosition = new Vector3(0, 0, 0);
    Vector3 shakePosition = new Vector3(0, 0, 0);
    float viewBobbingMult;
    float veiwBobbingCounter;
    float viewSmoothInOut = 1f;
    float viewShakeX;
    float viewShakeY;






    readonly float baseShake = 0.05f;
    public bool BobbingEnabled
    {
        get => bobbingEnabled;
        set => bobbingEnabled = value;
    }
    public bool ViewShakeEnabled
    {
        get => viewShakeEnabled;
        set => viewShakeEnabled = value;
    }

    void OnEnable() => PlayerHealth.OnHealthChanged += DoCameraShake;
    void OnDisable() => PlayerHealth.OnHealthChanged -= DoCameraShake;

    private void Start()
    {
        defaultRotation = parent.localRotation;

    }

    void FixedUpdate()
    {
        if (BobbingEnabled)
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

        // aim assist
        if (aimAssistEnabled)
        {
            // lol this sucks but whatever - designer code :)
            var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None)
                .OrderBy(e => Vector3.Distance(parent.position, e.transform.position))
                .ToArray();
            if (enemies.Length == 0) return;

            var closestEnemy = enemies[0];
            Vector3 directionToEnemy = (closestEnemy.transform.position - parent.position).normalized;
            float angleToEnemy = Vector3.Angle(parent.forward, directionToEnemy);
            Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);

            if (angleToEnemy < aimAssistAngleThreshold && Vector3.Distance(parent.position, closestEnemy.transform.position) < aimAssistMinDistance)
            {
                parent.rotation = Quaternion.Slerp(parent.rotation, targetRotation, Time.deltaTime * 2f);
                aimAssistTimer = aimAssistReset;
            }
            
            aimAssistTimer -= Time.deltaTime;
            if (aimAssistTimer <= 0)
            {
                float yaw = parent.eulerAngles.y;
                Quaternion targetNeutralRot = Quaternion.Euler(0f, yaw, 0f);

                parent.rotation = Quaternion.Slerp(
                    parent.rotation,
                    targetNeutralRot,
                    Time.deltaTime * 3f 
                );
            }
        }
    }

    private void DoCameraShake(float currentHealth, float previousHealth)
    {
        if (currentHealth - previousHealth < 0 && ViewShakeEnabled)
        {
            viewShakeX = (Random.value - 0.5f) * shakeIntensity * baseShake;
            viewShakeY = (Random.value - 0.5f) * shakeIntensity * baseShake;
        }
    }

    public void TriggerCameraShake(float intensity)
    {
        if (ViewShakeEnabled)
        {
            viewShakeX = (Random.value - 0.5f) * intensity * baseShake;
            viewShakeY = (Random.value - 0.5f) * intensity * baseShake;
        }
    }
}
