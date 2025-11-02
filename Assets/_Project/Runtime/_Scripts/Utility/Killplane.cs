using UnityEngine;

public class Killplane : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            if (other.TryGetComponent(out CharacterController controller))
            {
                controller.enabled = false;
                controller.transform.position = new Vector3(0, 30, 0);
                controller.enabled = true;
                return;
            }
            
            damageable.TakeDamage(float.MaxValue);
            Logger.LogWarning("Killplane activated on: " + other.name, this, "Killplane");

        }
        else
        {
            Logger.LogWarning("An object without IDamageable entered the killplane: " + other.name, this, "Killplane");
            Destroy(other.gameObject);
        }
    }
}
