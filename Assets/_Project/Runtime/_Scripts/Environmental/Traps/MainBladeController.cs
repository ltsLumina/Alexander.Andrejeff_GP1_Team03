// ChildCollisionReporter.cs
using UnityEngine;

public class ChildCollisionReporter : MonoBehaviour
{
    public System.Action<Collider, bool> onCollisionEvent;

    void OnTriggerEnter(Collider other)
    {
        onCollisionEvent?.Invoke(other, true);
    }

    void OnTriggerExit(Collider other)
    {
        onCollisionEvent?.Invoke(other, false);
    }
}
