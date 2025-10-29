using System;
using UnityEngine;

public class GameManager : SingletonPersistent<GameManager>
{
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
}
