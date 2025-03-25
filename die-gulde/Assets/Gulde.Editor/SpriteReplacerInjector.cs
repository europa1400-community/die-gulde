using Gulde.Client;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Gulde.Editor
{
    public class SpriteReplacerInjector : IProcessSceneWithReport
    {
        public int callbackOrder => 0;

        public void OnProcessScene(UnityEngine.SceneManagement.Scene scene, BuildReport report)
        {
            if (!BuildPipeline.isBuildingPlayer)
                return;
            
            var allRenderers = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.InstanceID);
            foreach (var sr in allRenderers)
            {
                if (sr.sprite == null) continue;

                var spritePath = AssetDatabase.GetAssetPath(sr.sprite);
                if (!spritePath.StartsWith("Assets/Editor/ConvertedAssets/Graphics"))
                    continue;

                var relativePath = spritePath["Assets/Editor/ConvertedAssets/Graphics".Length..].TrimStart('/');

                var replacer = sr.GetComponent<SpriteReplacer>();
                if (replacer == null)
                    replacer = sr.gameObject.AddComponent<SpriteReplacer>();

                replacer.relativePath = relativePath;

                sr.sprite = null;
            }
        }
    }
}
