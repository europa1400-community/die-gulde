using GuldeLib.Generators;
using GuldeLib.Names;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib
{
    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    public abstract class TypeObject<TObj> : SerializedScriptableObject where TObj : TypeObject<TObj>
    {
        [Generatable]
        [Optional]
        [ShowIf(nameof(HasNaming))]
        [BoxGroup("Naming")]
        [HideLabel]
        [OdinSerialize]
        public virtual GeneratableNaming Naming { get; set; }

        public virtual bool HasNaming => true;
    }
}