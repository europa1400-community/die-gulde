using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Europa1400.Tools.Pipeline;
using Gulde.Client;
using UnityEditor;
using UnityEngine;

namespace Gulde.Editor
{
    public class AssetPipelineToolWindow : EditorWindow
    {
        private string gamePath
        {
            get => EditorPrefs.GetString("AssetPipeline_GamePath", "");
            set => EditorPrefs.SetString("AssetPipeline_GamePath", value);
        }

        private string finalOutputPath => Path.Combine(Application.dataPath, "Editor/ConvertedAssets");
        private string tempOutputPath => Path.Combine(Application.persistentDataPath, "ConvertedAssets");

        private CancellationTokenSource? cts;
        private Task? pipelineTask;
        private int currentStep = 0;
        private PipelineProgress currentProgress;

        [MenuItem("Tools/Asset Pipeline")]
        public static void ShowWindow()
        {
            GetWindow<AssetPipelineToolWindow>("Asset Pipeline");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Game Path:");
            EditorGUILayout.BeginHorizontal();
            gamePath = EditorGUILayout.TextField(gamePath);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Game Path", "", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    gamePath = selectedPath;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Output Path:", finalOutputPath);

            GUI.enabled = pipelineTask == null || pipelineTask.IsCompleted;

            if (GUILayout.Button("Run Asset Pipeline"))
            {
                RunPipeline();
            }

            GUI.enabled = true;
        }

        private async void RunPipeline()
        {
            if (string.IsNullOrEmpty(gamePath) || !Directory.Exists(gamePath))
            {
                var selectedPath = EditorUtility.OpenFolderPanel("Select Game Path", "", "");
                if (string.IsNullOrEmpty(selectedPath))
                {
                    EditorUtility.DisplayDialog("Error", "Game path is required.", "OK");
                    return;
                }
                gamePath = selectedPath;
            }

            if (Directory.Exists(tempOutputPath))
            {
                Directory.Delete(tempOutputPath, true);
            }
            Directory.CreateDirectory(tempOutputPath);

            cts = new CancellationTokenSource();
            currentStep = 0;

            var progress = new Progress<PipelineProgress>(progress =>
            {
                if (currentProgress != progress)
                {
                    currentProgress = progress;
                    currentStep += 1;
                }
                
                var assetType = progress.Asset?.GetType().Name ?? "Asset";
                var progressTitle = $"{progress.Step} {assetType}";
                var info = $"[{progress.Current}/{progress.Total}] {progress.Asset?.RelativePath ?? string.Empty}";
                var pct = progress.Total > 0 ? (float)progress.Current / progress.Total : 0f;
                EditorUtility.DisplayProgressBar(progressTitle, info, pct);
            });

            try
            {
                pipelineTask = AssetTools.PrepareAllAsync(gamePath, tempOutputPath, progress, cts.Token);
                await pipelineTask;
                
                EditorUtility.ClearProgressBar();
            }
            catch (TaskCanceledException)
            {
                Debug.Log("Pipeline canceled.");
            }
            finally
            {
                cts.Dispose();
                DirectoryCopy(tempOutputPath, finalOutputPath, true);
                EditorUtility.FocusProjectWindow();
                AssetDatabase.Refresh();
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}
