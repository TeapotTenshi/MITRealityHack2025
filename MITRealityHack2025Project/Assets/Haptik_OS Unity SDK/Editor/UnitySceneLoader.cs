using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class UnitySceneLoader : EditorWindow
{
    [MenuItem("Haptikos/Built-in Scene Loader")]
    public static void ShowWindow()
    {
        UnitySceneLoader window = EditorWindow.GetWindow<UnitySceneLoader>();
        window.titleContent = new GUIContent("Built-in Scene Loader");
        window.Show();
    }

    private void OnGUI()
    {
        GUIStyle textAlignment = new GUIStyle("Button");
        textAlignment.alignment = TextAnchor.MiddleLeft;
        textAlignment.normal.textColor = Color.white;

        GUIStyle label1 = new GUIStyle(EditorStyles.boldLabel);
        label1.normal.textColor = StaticVariables.GREEN;
        GUILayout.Label("Available Scenes:", label1);

        foreach (var scene in EditorBuildSettings.scenes)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
            if (GUILayout.Button(sceneName, textAlignment))
            {
                LoadScene(sceneName);
            }
        }
    }

    private void LoadScene(string sceneName)
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(scene.path);
            if (name == sceneName)
            {
                EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
                break;
            }
        }
    }
}