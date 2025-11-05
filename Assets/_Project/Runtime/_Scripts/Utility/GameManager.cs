using System;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class GameManager : SingletonPersistent<GameManager>
{
	public int playerCoins;
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
}
