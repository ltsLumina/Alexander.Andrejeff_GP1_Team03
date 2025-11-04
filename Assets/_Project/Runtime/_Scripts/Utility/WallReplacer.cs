#pragma warning disable 0414

using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using VInspector;
#endif

public class WallReplacer : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] GameObject prefabWall;
    [Tooltip("The name (or part of it) of the walls to be replaced.")]
    [SerializeField] string wallNameToReplace;
    
    List<GameObject> newWalls = new ();
    
    [Button]
    public void ReplaceWall()
    {
        if (prefabWall == null)
        {
            Logger.LogWarning("Prefab wall is not set.", this, "WallReplacer");
            return;
        }
        
        if (string.IsNullOrEmpty(wallNameToReplace))
        {
            Logger.LogWarning("Wall name to replace is not set.", this, "WallReplacer");
            return;
        }
        
        newWalls.Clear();
        List<GameObject> walls = FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                                .Where(x => x != null && x.name.Contains(wallNameToReplace))
                                .ToList();
    
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Replace Walls");
    
        foreach (GameObject wall in walls)
        {
            // replace wall with prefabWall
            var newWall = Instantiate(prefabWall, wall.transform.position, wall.transform.rotation, wall.transform.parent);
            newWall.transform.localScale = prefabWall.transform.localScale;
            newWall.name = wall.name;
            Undo.RegisterCreatedObjectUndo(newWall, "Replace Wall");
            newWalls.Add(newWall);
    
            Undo.DestroyObjectImmediate(wall);
        }
    
        Undo.CollapseUndoOperations(group);
    }
#endif
}
