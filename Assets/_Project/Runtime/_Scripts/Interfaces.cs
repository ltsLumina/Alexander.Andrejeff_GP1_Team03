interface IDamageable
{
	void TakeDamage(float damage);
}

interface IInteractable
{
	void Interact();
}

public interface IInteractableObject
{
	InteractableType Type { get; }
}

