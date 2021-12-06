#if UNITY_EDITOR
using System;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace GuldeLib
{
    public class CreateObjectWindow : OdinEditorWindow
    {
        [OdinSerialize]
        [Required]
        [LabelText("File Name")]
        public string Name { get; set; }

        [OdinSerialize]
        [Required]
        [FolderPath]
        [LabelText("File Path")]
        public string Path { get; set; }

        string AssetName => $"{Name}.asset";
        string AssetPath => System.IO.Path.Combine(Path, AssetName);

        [OdinSerialize]
        [HideInInspector]
        SerializedScriptableObject Obj { get; set; }

        public event EventHandler<ObjectSavedEventArgs> Saved;

        public static CreateObjectWindow Open(SerializedScriptableObject obj)
        {
            var window = GetWindow<CreateObjectWindow>();
            window.Obj = obj;
            window.Show();

            return window;
        }

        [Button]
        public void Save()
        {
            AssetDatabase.CreateAsset(Obj, AssetPath);
            AssetDatabase.SaveAssets();

            Saved?.Invoke(this, new ObjectSavedEventArgs(Obj));
        }
    }
}
#endif