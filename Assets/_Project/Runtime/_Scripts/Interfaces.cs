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

public interface IInteractable
{
	void Interact();
	InteractableType InteractableType => InteractableType.Default;
}

