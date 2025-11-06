using UnityEngine;

public class Killplane : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(99);
            Logger.LogWarning("Killplane activated on: " + other.name, this, "Killplane");

        }
        else
        {
            Logger.LogWarning("An object without IDamageable entered the killplane: " + other.name, this, "Killplane");
            Destroy(other.gameObject);
        }
    }
}
