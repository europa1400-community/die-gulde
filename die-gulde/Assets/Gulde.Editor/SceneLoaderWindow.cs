using System.IO;
using Gulde.Client;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gulde.Editor
{
    public class SceneLoaderWindow : EditorWindow
    {
        string basePath = "";
        string scenePath = "";

        [MenuItem("Tools/Scene Loader")]
        public static void ShowWindow()
        {
            GetWindow<SceneLoaderWindow>("Scene Loader");
        }

        void OnGUI()
        {
            GUILayout.Label("Scene Loader", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Base Path:", GUILayout.Width(80));
            basePath = GUILayout.TextField(basePath, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Scene Path:", GUILayout.Width(80));
            scenePath = GUILayout.TextField(scenePath, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Load"))
            {
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                scene.name = sceneName;
                
                SceneLoader.LoadScene(basePath, scenePath);
            }
        }
    }
}
