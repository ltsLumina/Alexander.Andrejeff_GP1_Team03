using System;
using System.Collections;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class GameManager : SingletonPersistent<GameManager>
{
	public int playerCoins;

	Sound ambientCry;
	Sound ambientShriek;
	
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		ambientCry = new Sound(SFX.AmbientCry);
		ambientCry.SetVolume(0.4f);
		ambientCry.SetSpatialSound();
		ambientCry.SetFollowTarget(FindFirstObjectByType<PlayerController>().transform);
		StartCoroutine(CaveNoise());
		
		ambientShriek = new Sound(SFX.AmbientShriek);
		ambientShriek.SetVolume(0.35f);
		ambientShriek.SetSpatialSound();
		ambientShriek.SetFollowTarget(FindFirstObjectByType<PlayerController>().transform);
		StartCoroutine(ShriekNoise());
	}

	IEnumerator CaveNoise()
	{
		yield return new WaitForSeconds(60f);
		
		ambientCry.Play();
	}
	
	IEnumerator ShriekNoise()
	{
		yield return new WaitForSeconds(180f);

		ambientShriek.Play();
	}
}
