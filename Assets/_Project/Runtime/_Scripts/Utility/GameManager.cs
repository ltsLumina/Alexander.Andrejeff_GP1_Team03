using System;
using UnityEngine;

public class GameManager : SingletonPersistent<GameManager>
{
	public int playerCoins = 0;
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
}
