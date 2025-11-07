using MelenitasDev.SoundsGood;
using UnityEngine;
using VInspector;

[SelectionBase]
public class Crate : MonoBehaviour, IDamageable
{
	[Header("Crate"), Tooltip("Can be broken on impact or by attacking it.")]
	[SerializeField] bool breakable;
	[ShowIf(nameof(breakable), true)]
	[SerializeField] int health = 1;
	[EndIf]
	
	[Tooltip("If true, the crate can be broken on impact after kicking it. (Mostly for doors)")]
	[SerializeField] bool kickable;
	[ShowIf(nameof(kickable), true)]
	[Tooltip("The velocity threshold at which the crate will break upon collision.")]
	[SerializeField] float breakVelocity = 5f;
	[EndIf]

	[Header("VFX")]
	[SerializeField] ParticleSystem dustVFX;
	[SerializeField] ParticleSystem breakVFX;

	public Rigidbody rb;
	
	Sound breakSound;
	public Sound kickSound;
	Sound hitSound;
	
	public bool Breakable => breakable;

#if UNITY_EDITOR
	// void OnDrawGizmos()
	// {
	// 	if (!Application.isPlaying) return;
	// 	if (Vector3.Distance(transform.position, SceneView.lastActiveSceneView.camera.transform.position) > 20f) return;
	//
	// 	// string healthInfo = Breakable ? $"Health: {health}" : "Unbreakable";
	// 	//string velocityInfo = $"Velocity: {rb.linearVelocity.magnitude:F2}";
	//
	// 	//Handles.Label(transform.position + Vector3.up, $"[{name}]\n{velocityInfo}");
	// }
#endif
	
	void Start()
	{
		rb = GetComponent<Rigidbody>();

		#region Init Sounds
		breakSound = new (SFX.CrateBreak);
		kickSound = new (SFX.CrateKick);
		hitSound = new (SFX.CrateHit);

		#region Break
		breakSound.SetOutput(Output.SFX);
		breakSound.SetVolume(0.15f);
		breakSound.SetRandomPitch(new (0.85f, 1.05f));
		breakSound.SetSpatialSound();
		breakSound.SetHearDistance(3f, 6f);
		breakSound.SetFollowTarget(transform);
		#endregion

		#region Kick
		kickSound.SetOutput(Output.SFX);
		kickSound.SetVolume(0.9f);
		kickSound.SetRandomPitch(new (0.95f, 1.05f));
		kickSound.SetSpatialSound();
		kickSound.SetDopplerLevel(1);
		kickSound.SetFollowTarget(transform);
		#endregion

		#region Hit
		hitSound.SetOutput(Output.SFX);
		hitSound.SetVolume(0.75f);
		hitSound.SetRandomPitch();
		hitSound.SetSpatialSound();
		hitSound.SetDopplerLevel(1);
		hitSound.SetHearDistance(3f, 15f);
		hitSound.SetFollowTarget(transform);
		#endregion
		#endregion
	}

	public void TakeDamage(float damage, DamageSource source = DamageSource.Crate)
	{
		if (!Breakable) return;
		health -= (int)damage;
		if (health <= 0f) Break();
	}

	void Break()
	{
		breakSound.Play();
		Instantiate(breakVFX, transform.position, transform.rotation);
		gameObject.SetActive(false);
		Destroy(gameObject, 0.1f);
	}
	
	void OnCollisionEnter(Collision other)
	{
		if (rb.linearVelocity.magnitude > 2f)
		{
			hitSound.Play();
			Vector3 offset = Vector3.up * 0.85f;
			Instantiate(dustVFX, transform.position - offset, Quaternion.identity);

			if (other.gameObject.TryGetComponent(out Enemy enemy))
			{
				enemy.TakeDamage(1, DamageSource.Crate);
				Break();
				return;
			}
		}

		if (rb.linearVelocity.magnitude >= breakVelocity)
		{
			if (Breakable) TakeDamage(1f);
			if (kickable) Break();
		}
	}
}
