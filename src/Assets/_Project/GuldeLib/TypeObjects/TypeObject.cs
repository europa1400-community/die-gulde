using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    public abstract class TypeObject<TObj> : SerializedScriptableObject where TObj : TypeObject<TObj>
    {
        [ShowIf(nameof(SupportsNaming))]
        [Naming]
        [OdinSerialize]
        public bool HasNaming { get; set; }

        [ShowIf(nameof(SupportsNaming))]
        [Optional]
        [Naming]
        [OdinSerialize]
        public virtual GeneratableNaming Naming { get; set; }

        public abstract bool SupportsNaming { get; }

        [Optional]
        [Setting]
        [OdinSerialize]
        public virtual LogType LogLevel { get; set; } = LogType.Warning;
    }
}