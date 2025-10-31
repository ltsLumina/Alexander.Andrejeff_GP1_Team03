using UnityEngine;

public class RotatingBridge : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject bridgeObject;

    public void Interact() => RotateBridge();

    private void RotateBridge()
    {
        bridgeObject.transform.Rotate(0f, 180f, 0f);
    }
}
