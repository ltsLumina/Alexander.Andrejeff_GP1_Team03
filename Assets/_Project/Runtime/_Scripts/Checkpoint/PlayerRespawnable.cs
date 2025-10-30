// ---------- Player ----------
using UnityEngine;

public class PlayerRespawnable : MonoBehaviour
{
    CharacterController cc; // or your controller
    Rigidbody rb;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    public void TeleportTo(Vector3 pos, Quaternion rot)
    {
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (cc && cc.enabled)
        {
            cc.enabled = false;
            transform.SetPositionAndRotation(pos, rot);
            cc.enabled = true;
        }
        else
        {
            transform.SetPositionAndRotation(pos, rot);
        }
    }
}
