public enum DamageSource
{
	Player,
	Enemy,
	Crate,
	Trap,
}

interface IDamageable
{
	void TakeDamage(float damage, DamageSource source = DamageSource.Player);
}

interface IInteractable
{
	void Interact();
}

public interface IInteractableObject
{
	InteractableType Type { get; }
}

