using System.IO;
using UnityEngine;

namespace Gulde.Client
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteReplacer : MonoBehaviour
    {
        private SpriteRenderer SpriteRenderer => GetComponent<SpriteRenderer>();

        public string relativePath;

        void Awake()
        {
            var path = Path.Combine(Application.persistentDataPath, "ConvertedAssets", "Graphics", relativePath);
            
            if (!Path.HasExtension(path) || Path.GetExtension(path) != ".png")
                path += ".png";
            
            if (!File.Exists(path))
            {
                Debug.LogError($"File not found: {path}");
                return;
            }

            var texture = new Texture2D(1, 1);
            var fileData = File.ReadAllBytes(path);
            texture.LoadImage(fileData);
            
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            SpriteRenderer.sprite = sprite;
        }
    }
}