using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace GuldeLib.Generators
{
    public abstract class GeneratableTypeObject<TObj> : Generatable<TObj> where TObj : TypeObject<TObj>
    {

#if UNITY_EDITOR
        protected override bool IsTemporary => Value && AssetDatabase.GetAssetPath(Value) == string.Empty;

        protected override bool IsSavable => true;

        public override void Save()
        {
            var window = CreateObjectWindow.Open(Value);
            window.Saved += OnSaved;
        }

        void OnSaved(object sender, ObjectSavedEventArgs e)
        {
            if (!(e.Obj is TObj tObj)) return;

            Value = tObj;
        }
#endif
    }
}