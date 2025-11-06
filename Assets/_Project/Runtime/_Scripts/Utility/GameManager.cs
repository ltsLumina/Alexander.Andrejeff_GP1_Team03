#define INCLUDE_DEBUG_KEYS

using System.Collections;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : SingletonPersistent<GameManager>
{
	[SerializeField] AudioMixer mixer;
	[SerializeField] WeaponData dagger;
	[SerializeField] WeaponData staff;
	
	public int playerCoins;

	Sound ambientCry;
	Sound ambientShriek;

	void Start()
	{
		Debug.Assert(mixer != null, "AudioMixer is not assigned in GameManager.");
		Debug.Assert(dagger != null, "Dagger WeaponData is not assigned in GameManager.");
		Debug.Assert(staff != null, "Staff WeaponData is not assigned in GameManager.");
		
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		ambientCry = new Sound(SFX.AmbientCry);
		ambientCry.SetVolume(0.2f);
		ambientCry.SetSpatialSound();
		StartCoroutine(CaveNoise());
		
		ambientShriek = new Sound(SFX.AmbientShriek);
		ambientShriek.SetVolume(0.2f);
		ambientShriek.SetSpatialSound();
		StartCoroutine(ShriekNoise());
		
		StartCoroutine(FixCrateNoise());
	}

#if INCLUDE_DEBUG_KEYS
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			var player = FindFirstObjectByType<PlayerController>();
			player.Weapon.Equip(dagger);
		}

		if (Input.GetKeyDown(KeyCode.F2))
		{
			var player = FindFirstObjectByType<PlayerController>();
			player.Weapon.Equip(staff);
		}
		
		if (Input.GetKeyDown(KeyCode.F3))
		{
			var player = FindFirstObjectByType<PlayerController>();
			player.GetComponent<PlayerHealth>().TakeDamage(99);
		}

		if (Input.GetKeyDown(KeyCode.F4))
		{
			SceneManager.LoadScene(0);
		}
	}
#endif

	IEnumerator CaveNoise()
	{
		yield return new WaitForSeconds(60f);
		
		ambientCry.SetPosition(FindFirstObjectByType<PlayerController>().transform.position);
		ambientCry.Play();
	}
	
	IEnumerator ShriekNoise()
	{
		yield return new WaitForSeconds(180f);

		ambientShriek.SetPosition(FindFirstObjectByType<PlayerController>().transform.position);
		ambientShriek.Play();
	}


	IEnumerator FixCrateNoise()
	{
		// set SFX to 0 for 1 second
		SoundsGoodManager.ChangeOutputVolume(Output.SFX, 0f);
		yield return new WaitForSeconds(1f);

		// restore previous value
		SoundsGoodManager.ChangeOutputVolume(Output.SFX, 1f);
	}
}
