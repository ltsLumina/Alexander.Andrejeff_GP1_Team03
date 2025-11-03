using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Networking;
#endif

#if UNITY_EDITOR
namespace LightShaft.PFH
{
    [InitializeOnLoad]
    public class PlayFromHere : Editor
    {
        // Key that's got to be pressed when right clicking, to get a context menu.
        static readonly EventModifiers modifier = EventModifiers.Shift;
        const float DEFAULT_CREATION_DISTANCE = 500f;

        static PlayFromHere()
        {
            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }


        private static void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.EnteredEditMode)
            {
                PlayFromHereConfig config = Resources.Load("Config/PFH_Config") as PlayFromHereConfig;
                if (config.playedFromPos)
                {
                    config.playedFromPos = false;
                    GameObject player;
                    if (PFHPlayer.instance == null)
                    {
                        player = GameObject.FindGameObjectWithTag("Player");
                        if (player == null)
                        {
                            Debug.LogWarning("Don't found any object with the tag 'Player'");
                            return;
                        }
                    }
                    else
                    {
                        player = PFHPlayer.instance.gameObject;
                    }

                    player.transform.position = config.tempPlayerPosition;
                    player.transform.rotation = config.tempPlayerRotation;
                    EditorUtility.SetDirty(config);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        static void OnSceneGUI(SceneView sceneview)
        {
            if (Event.current.modifiers == modifier && Event.current.button == 1 && Event.current.type == EventType.MouseDown)
            {
                // Ray from editor camera.
                var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                // Check if object selected.
                var materialIndex = 0;
                var go = HandleUtility.PickGameObject(Event.current.mousePosition, out materialIndex);

                if (go != null)
                {
                    ShowMenu(go, ray);
                    return;
                }
            }
        }

        static void ShowMenu(GameObject go, Ray ray)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Play From Here"), false, () => PlayFrom(go, ray));
            menu.AddItem(new GUIContent("UNREAL SUPERIOR, UNITY INFERIOR"), true, () => Debug.Log("Unreal is cooler than Unity"));
            // open url
            menu.AddItem
            (new GUIContent("Trausti leg day skips: 3"), false,
             () => Application.OpenURL
                     ("https://cdn-images.threadless.com/~tms/pugsgym/products/226621/shirt-1494524489-fcd85510a697255ab073981992eedc98.png?v=3&d=g6NvcHOWk6R0cmltksLCgJOmcmVzaXplkIOld2lkdGjLQI8gAAAAAACmaGVpZ2h0y0CPIAAAAAAAqGFsbG93X3VwwpOvY2FudmFzX2NlbnRlcmVkks0EsM0EsIGqYmFja2dyb3VuZKZlYjU3MmaTpnJlc2l6ZZHNAyCAk69jYW52YXNfY2VudGVyZWSTzQMgzQMgpyNmZmZmZmaAk6ZlbmNvZGWSpHdlYnBVgKVmb3JjZcKpb25seV9tZXRhwg==&f=mp")); 
            menu.ShowAsContext();
        }

        private static void PlayFrom(GameObject go, Ray ray)
        {
            PlayFromHereConfig config = Resources.Load("Config/PFH_Config") as PlayFromHereConfig;
            GameObject player;
            if (PFHPlayer.instance == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                if (player == null)
                {
                    Debug.LogWarning("Don't found any object with the tag 'Player'");
                    return;
                }
            }
            else
            {
                player = PFHPlayer.instance.gameObject;
            }

            config.tempPlayerPosition = player.transform.position;
            config.tempPlayerRotation = player.transform.rotation;

            RaycastHit hitInfo;
            var position = Vector3.zero;

            if (Physics.Raycast(ray, out hitInfo, float.MaxValue))
            {
                position = hitInfo.point;
            }
            else
            {
                position = ray.GetPoint(DEFAULT_CREATION_DISTANCE);
            }

            Vector3 newPosition = new Vector3(
                config.keepXPosition ? player.transform.position.x : position.x,
                config.keepYPosition ? player.transform.position.y : position.y,
                config.keepZPosition ? player.transform.position.z : position.z);

            player.transform.position = newPosition + config.offset;


            if (config.rotatePlayerWithView)
            {
                SceneView sceneCam = SceneView.lastActiveSceneView;
                player.transform.eulerAngles = new Vector3(player.transform.eulerAngles.x, sceneCam.rotation.eulerAngles.y, player.transform.eulerAngles.z);
            }
            config.playedFromPos = true;
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();

            EditorApplication.isPlaying = true;
        }

    }
}

#endif