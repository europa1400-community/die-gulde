using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

namespace GuldeLib.Generators
{
    [Serializable]
    [GUIColor("@$value != null ? $value.GenerationIndicatorColor : Color.white")]
    public abstract class Generatable<TObj>
    {
        [OdinSerialize]
        [PropertyOrder(float.NegativeInfinity)]
        [BoxGroup("Generation")]
        public virtual bool IsGenerated { get; set; } = true;


#if UNITY_EDITOR
        string GroupName(InspectorProperty property) => property.NiceName;
        // [DetailedInfoBox(
        //     "Temporary",
        //     "This object is temporary and will be deleted when the domain is reloaded",
        //     InfoMessageType.Warning,
        //     nameof(IsTemporary))]
#endif
        [OdinSerialize]
        [BoxGroup("Generation")]
        public virtual TObj Value { get; set; }

        protected virtual bool SupportsDefaultGeneration => true;

        public virtual bool IsTemporary => false;

        protected virtual bool IsSavable => false;

        protected bool HasValue => Value != null;

        protected virtual bool IsNullable => default(TObj) == null;

        protected virtual bool IsValid => true;

        bool IsSavingEnabled => IsSavable && IsTemporary;

        Color RedIndicatorColor => new Color(1f, 0.7f, 0.7f, 1f);
        Color GreenIndicatorColor => new Color(0.7f, 1f, 0.7f, 1f);
        Color BlueIndicatorColor => new Color(0.7f, 0.7f, 1f, 1f);
        Color YellowIndicatorColor => new Color(1f, 1f, 0.7f, 1f);

        public virtual Color GenerationIndicatorColor => IsValid switch
        {
            true => IsTemporary switch
            {
                true => YellowIndicatorColor,
                false => IsGenerated switch
                {
                    true => BlueIndicatorColor,
                    false => IsNullable switch
                    {
                        true => HasValue switch
                        {
                            true => GreenIndicatorColor,
                            false => RedIndicatorColor,
                        },
                        false => GreenIndicatorColor,
                    },
                },
            },
            false => RedIndicatorColor,
        };

        [PropertyTooltip("@SupportsDefaultGeneration ? \"\" : \"This Generatable has upwards dependencies and can therefore not be generated from the inspector.\"")]
        [EnableIf(nameof(SupportsDefaultGeneration))]
        [Button("Generate", ButtonSizes.Medium)]
        [HorizontalGroup("Generation/Actions")]
        [NoGUIColor]
        public virtual void Generate()
        {
            Value = default(TObj);
        }

        [PropertyTooltip("Resets the value currently held by this Generatable.")]
        [Button("Remove", ButtonSizes.Medium)]
        [HorizontalGroup("Generation/Actions")]
        [NoGUIColor]
        public virtual void Remove()
        {
            Value = default(TObj);
        }

        [PropertyTooltip("@SaveTooltip")]
        [EnableIf(nameof(IsSavingEnabled))]
        [Button("Save", ButtonSizes.Medium)]
        [HorizontalGroup("Generation/Actions")]
        [NoGUIColor]
        public virtual void Save()
        {
        }

        string SaveTooltip => IsSavingEnabled switch
        {
            true => "Save this temporary value in an asset.",
            false => IsNullable switch
            {
                true => IsSavable switch
                {
                    true => HasValue switch
                    {
                        true => "This object is already saved in an asset.",
                        false => "This object can not be saved right now.",
                    },
                    false => "This object does not support saving.",
                },
                false => "This object is of value type and is therefore already serialized within the parent object.",
            },
        };
    }
}