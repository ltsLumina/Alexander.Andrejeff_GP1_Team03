using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GUILayout;

public class LevelSelector : EditorWindow
{
	[MenuItem("Tools/Level Selector")]
	static void ShowWindow()
	{
		var window = GetWindow<LevelSelector>();
		window.minSize = new Vector2(200, 300);
		window.titleContent = new ("Level Selector");
		window.Show();
	}

	List<string> tags;
	bool playFromHere;
	readonly Dictionary<string, int> tagIndices = new ();
	GameObject levelPrefab;

	void Awake()
	{
		tags = InternalEditorUtility.tags.ToList();
		tags.Remove("Untagged");
		tags.RemoveRange(0, 7); // Remove default Unity tags
	}

	void OnGUI()
	{
		using (new EditorGUILayout.VerticalScope("box"))
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				FlexibleSpace();
				EditorGUILayout.LabelField("Level Selector", EditorStyles.boldLabel);
				FlexibleSpace();
			}

			using (new EditorGUILayout.HorizontalScope("box"))
			{
				using (new EditorGUILayout.VerticalScope())
				{
					playFromHere = EditorGUILayout.ToggleLeft("Set player position to Selected Level root", playFromHere, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter }, Width(400), Height(20)); 
					if (Button(("Select Player"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter }, Width(100), Height(30)))
					{
						var player = FindFirstObjectByType<PlayerController>();
						if (player != null)
						{
							EditorGUIUtility.PingObject(player.gameObject);
							Selection.activeObject = player.gameObject;
						}
					}
				}
				
			}

			foreach (var tag in tags)
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					if (Button(tag, new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter }, Width(200), Height(50)))
					{
						var objs = GameObject.FindGameObjectsWithTag(tag);

						if (objs == null || objs.Length == 0) { Debug.LogWarning("No objects found for tag: " + tag); }
						else
						{
							tagIndices.TryGetValue(tag, out int idx);
							idx %= objs.Length;
							levelPrefab = objs[idx];

							// advance index for next press
							tagIndices[tag] = (idx + 1) % objs.Length;

							// out of range
							if (idx >= objs.Length) { tagIndices[tag] = 0; }

							if (levelPrefab != null)
							{
								EditorGUIUtility.PingObject(levelPrefab);
								Selection.activeObject = levelPrefab;
								SceneView.lastActiveSceneView.LookAt(levelPrefab.transform.position + new Vector3(5, 5, -5));

								if (playFromHere)
								{
									var player = FindFirstObjectByType<PlayerController>();
									if (!player) return;

									Undo.RecordObject(player.transform, "Move Player");
									player.transform.position = levelPrefab.transform.position + Vector3.up;
								}
							}
						}
					}

					using (new EditorGUILayout.VerticalScope("box"))
					{
						bool hasLevel = levelPrefab != null && levelPrefab.CompareTag(tag);
						string currentRoom = hasLevel ? levelPrefab.name : "None";
						EditorGUILayout.LabelField($"Current Room: {currentRoom}", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
						EditorGUILayout.LabelField($"Index: {tagIndices.GetValueOrDefault(tag, 0)}/{GameObject.FindGameObjectsWithTag(tag).Length - 1}", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
					}

					FlexibleSpace();
				}
			}
		}

		EditorGUILayout.LabelField("", new GUIStyle(GUI.skin.horizontalSlider));

		using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
		{
			using (new EditorGUILayout.VerticalScope("box"))
			{
				var sceneCount = SceneManager.sceneCountInBuildSettings;

				for (var i = 0; i < sceneCount; i++)
				{
					var niceName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));

					if (Button(niceName, new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter }, Width(200), Height(30)))
					{
						bool wantsToSave = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
						if (wantsToSave) EditorSceneManager.SaveOpenScenes();
						EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(i), OpenSceneMode.Single);
					}
				}
			}
		}
	}
}
