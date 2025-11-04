//#define ENABLE_UNREAL_SUPERIOR

#region
#if ENABLE_UNREAL_SUPERIOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
#endif
using UnityEditor;
#endregion

[InitializeOnLoad]
public static class UnrealSuperior
{
#if ENABLE_UNREAL_SUPERIOR
	const string EditorPrefsKey = "UnrealSuperior_Paths"; // semicolon-separated custom paths
	readonly static string[] DefaultRoots = { @"C:\Program Files\Epic Games", @"C:\Program Files (x86)\Epic Games" };

	static UnrealSuperior() { EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged; }

	static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange state)
	{
		if (state != PlayModeStateChange.EnteredPlayMode) return;
		EditorApplication.ExitPlaymode();

		List<string> candidates = GetCandidatePaths().ToList();

		if (candidates.Count == 0)
		{
			throw new InvalidOperationException("No Unreal Editor paths configured or discovered. Please set at least one valid path in EditorPrefs under key: " + EditorPrefsKey);
		}

		foreach (string path in candidates)
		{
			if (!File.Exists(path)) continue;

			try
			{
				Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
				return;
			} catch (Exception ex) { throw new Exception($"Error starting '{path}': {ex.Message}"); }
		}

		throw new FileNotFoundException("None of the configured or discovered Unreal Editor paths exist:\n" + string.Join("\n", candidates));
	}

	static IEnumerable<string> GetCandidatePaths()
	{
		var list = new List<string>();

		// 1) custom user-provided paths via EditorPrefs (semicolon-separated)
		if (EditorPrefs.HasKey(EditorPrefsKey))
		{
			string raw = EditorPrefs.GetString(EditorPrefsKey, "").Trim();
			if (!string.IsNullOrEmpty(raw)) { list.AddRange(raw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim())); }
		}

		// 2) auto-discover under common Epic Games install roots (look for UE_* folders)
		foreach (string root in DefaultRoots)
		{
			try
			{
				if (!Directory.Exists(root)) continue;

				IEnumerable<string> dirs = Directory.EnumerateDirectories(root, "UE_*", SearchOption.TopDirectoryOnly);

				// prefer higher-version names last by ordering descending
				foreach (string dir in dirs.OrderByDescending(d => d))
				{
					string exe = Path.Combine(dir, "Engine", "Binaries", "Win64", "UnrealEditor.exe");
					list.Add(exe);
				}
			} catch (Exception ex) { throw new Exception($"Error enumerating Unreal Engine installations under '{root}': {ex.Message}"); }
		}

		// 3) de-duplicate while preserving order
		return list.Where(p => !string.IsNullOrWhiteSpace(p)).Select(Path.GetFullPath).Distinct();
	}
#endif
}
