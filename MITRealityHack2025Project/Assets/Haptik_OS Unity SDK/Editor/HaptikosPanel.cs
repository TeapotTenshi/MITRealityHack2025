using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UIElements;

#if UNITY_EDITOR

class HaptikosPanel : EditorWindow
{
    HaptikosItemPanelSettings itemPanelSettings;
    bool playerbool = false, haptikosAssets = false, grabbables = false, advanced = false, gestures = false, extras = false;

    private void OnEnable()
    {
        itemPanelSettings = (HaptikosItemPanelSettings)AssetDatabase.LoadAssetAtPath("Assets/Haptik_OS Unity SDK/Editor/Haptikos Item Panel Settings.asset", typeof(HaptikosItemPanelSettings));
    }

    [MenuItem("Haptikos/Haptikos Panel")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<HaptikosPanel>("Haptikos Item Panel");

        window.maxSize = new Vector2(200, 650);

        window.minSize = new Vector2(200, 650);
    }

    void OnGUI()
    {
        if (itemPanelSettings)
        {
            GUIStyle textAlignment = new GUIStyle("Button");
            textAlignment.alignment = TextAnchor.MiddleLeft;
            textAlignment.normal.textColor = Color.white;

            GUIStyle label = new GUIStyle(EditorStyles.foldoutHeader);
            label.normal.textColor = StaticVariables.GREEN;
            playerbool = EditorGUILayout.Foldout(playerbool,"Player", true,label);
            if (playerbool)
            {
                GUILayout.BeginVertical("Box");

                GameObject player = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.haptikosPlayerPath, typeof(GameObject));
                if (GUILayout.Button("Haptikos Player", textAlignment))
                {
                    Debug.Log("Haptikos Player is added to the scene!");
                    PrefabUtility.InstantiatePrefab(player);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }

                GameObject playbackPlayer = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.haptikosPlaybackPlayerPath, typeof(GameObject));
                if (GUILayout.Button("Playback Hand", textAlignment))
                {
                    Debug.Log("Playback Hand is added to the scene!");
                    PrefabUtility.InstantiatePrefab(playbackPlayer);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }

                GUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            haptikosAssets = EditorGUILayout.Foldout(haptikosAssets,"Haptikos Assets", true, label);
            if (haptikosAssets)
            {
                GUILayout.BeginVertical("Box");

                GameObject button = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.buttonPath, typeof(GameObject));
                if (GUILayout.Button("Table Button", textAlignment))
                {
                    Debug.Log("Haptikos Button was added to the scene");
                    PrefabUtility.InstantiatePrefab(button);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }

                GameObject slider = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.sliderPath, typeof(GameObject));
                if (GUILayout.Button("Slider", textAlignment))
                {
                    Debug.Log("Haptikos Slider was added to the scene");
                    PrefabUtility.InstantiatePrefab(slider);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }

                GameObject dimmer = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.dimmerPath, typeof(GameObject));
                if (GUILayout.Button("Dimmer", textAlignment))
                {
                    Debug.Log("Haptikos Dimmer was added to the scene");
                    PrefabUtility.InstantiatePrefab(dimmer);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }

                GameObject lever = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.leverPath, typeof(GameObject));
                if (GUILayout.Button("Lever", textAlignment))
                {
                    Debug.Log("Haptikos Lever was added to the scene");
                    PrefabUtility.InstantiatePrefab(lever);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }

                GameObject toggleSwitch = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.switchPath, typeof(GameObject));
                if (GUILayout.Button("Table Switch", textAlignment))
                {
                    Debug.Log("Haptikos Table Switch was added to the scene");
                    PrefabUtility.InstantiatePrefab(toggleSwitch);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }

                GameObject wallSwitch = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.wallSwitchPath, typeof(GameObject));
                if (GUILayout.Button("Wall Switch", textAlignment))
                {
                    Debug.Log("Haptikos Wall Switch was added to the scene");
                    PrefabUtility.InstantiatePrefab(wallSwitch);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }

                GUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            grabbables =  EditorGUILayout.Foldout(grabbables,"Grabbable 3D Assets", true, label);
            if (grabbables)
            {
                GUILayout.BeginVertical("Box");

                GameObject cube = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.basicGrabbablePath, typeof(GameObject));
                if (GUILayout.Button("Basic Grabbable", textAlignment))
                {
                    Debug.Log("Haptikos Cube was added to the scene");
                    PrefabUtility.InstantiatePrefab(cube);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }

                GUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            
            advanced = EditorGUILayout.Foldout(advanced,"Advanced Premade Haptic Items", true, label);
            if (advanced)
            {
                GUILayout.BeginVertical("Box");

                GameObject beatingHeart = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.beatingHeartPath, typeof(GameObject));
                if (GUILayout.Button("Beating Heart", textAlignment))
                {
                    Debug.Log("Beating Heart was added to the scene");
                    PrefabUtility.InstantiatePrefab(beatingHeart);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }

                GameObject waterSink = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.waterSinkPath, typeof(GameObject));
                if (GUILayout.Button("Water Sink", textAlignment))
                {
                    Debug.Log("Water Sink was added to the scene");
                    PrefabUtility.InstantiatePrefab(waterSink);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }


                GUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            

            gestures = EditorGUILayout.Foldout(gestures,"Haptikos Gesture Prefabs", true, label);
            if (gestures)
            {
                GUILayout.BeginVertical("Box");

                GameObject recognizer = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.recognizerPath, typeof(GameObject));
                if (GUILayout.Button("Gesture Recognizer", textAlignment))
                {
                    Debug.Log("Haptikos Recognizer was added to the scene");
                    PrefabUtility.InstantiatePrefab(recognizer);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }


                GameObject teleportRaycaster = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.teleportRaycasterPath, typeof(GameObject));
                if (GUILayout.Button("Teleport Raycaster", textAlignment))
                {
                    Debug.Log("Haptikos Teleport Raycaster was added to the scene");
                    PrefabUtility.InstantiatePrefab(teleportRaycaster);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }


                GameObject cursorRaycaster = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.cursorRaycasterPath, typeof(GameObject));
                if (GUILayout.Button("Cursor Raycaster", textAlignment))
                {
                    Debug.Log("Haptikos Cursor Raycaster was added to the scene");
                    PrefabUtility.InstantiatePrefab(cursorRaycaster);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }


                GameObject teleportArea = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.teleportAreaPath, typeof(GameObject));
                if (GUILayout.Button("Teleport Area", textAlignment))
                {
                    Debug.Log("Haptikos Teleport Area was added to the scene");
                    PrefabUtility.InstantiatePrefab(teleportArea);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }


                GameObject canvasAndButton = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.uiButtonPath, typeof(GameObject));
                if (GUILayout.Button("Raycast Canvas and Button", textAlignment))
                {
                    Debug.Log("Haptikos UI Canvas and Button was added to the scene");
                    PrefabUtility.InstantiatePrefab(canvasAndButton);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }

                GameObject followRay = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.followRayPath, typeof(GameObject));

                if (GUILayout.Button("Follow Ray Object", textAlignment))
                {
                    Debug.Log("Haptikos Follow Ray Object was added to the scene");
                    PrefabUtility.InstantiatePrefab(followRay);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }

                GameObject raycastMenu = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.raycastMenuPath, typeof(GameObject));
                if (GUILayout.Button("Raycast Menu", textAlignment))
                {
                    Debug.Log("Haptikos Raycast Menu was added to the scene");
                    PrefabUtility.InstantiatePrefab(raycastMenu);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }

                GUILayout.EndVertical();
                EditorGUILayout.Space();

            }


            extras = EditorGUILayout.Foldout(extras, "Extras", true, label);
            if (extras)
            {
                GUILayout.BeginVertical("Box");

                GameObject teleporter = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.teleportationAreaPath, typeof(GameObject));
                if (GUILayout.Button("Teleporter", textAlignment))
                {
                    Debug.Log("Teleporter was added to the scene");
                    PrefabUtility.InstantiatePrefab(teleporter);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }


                GameObject portal = (GameObject)AssetDatabase.LoadAssetAtPath(itemPanelSettings.portalPath, typeof(GameObject));
                if (GUILayout.Button("Portal", textAlignment))
                {
                    Debug.Log("Portal was added to the scene");
                    PrefabUtility.InstantiatePrefab(portal);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }




                GUILayout.EndVertical();
                EditorGUILayout.Space();

            }
        }
        else
        {
            Debug.Log("There are no Panel Settings available!");
        }
    }

}

#endif