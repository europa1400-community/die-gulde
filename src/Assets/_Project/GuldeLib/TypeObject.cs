using GuldeLib.Generators;
using GuldeLib.Names;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace GuldeLib
{
    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    public abstract class TypeObject<TObj> : SerializedScriptableObject where TObj : TypeObject<TObj>
    {
        [Generatable]
        [Optional]
        [ShowIf(nameof(HasNaming))]
        [HideLabel]
        [OdinSerialize]
        public virtual GeneratableNaming Naming { get; set; }

        public virtual bool HasNaming => false;

#if UNITY_EDITOR
        protected string PropertyName(InspectorProperty property) => property.NiceName;
#endif
    }
}