using System;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace GuldeLib.Generators
{
    [Serializable]
    public abstract class Generatable<TObj>
    {
        [OdinSerialize]
        [PropertyOrder(float.NegativeInfinity)]
        [BoxGroup("Generation")]
        public virtual bool IsGenerated { get; set; } = true;

        string GroupName(InspectorProperty property) => property.NiceName;

#if UNITY_EDITOR
        [DetailedInfoBox(
            "Temporary",
            "This object is temporary and will be deleted when the domain is reloaded",
            InfoMessageType.Warning,
            nameof(IsTemporary))]
#endif
        [OdinSerialize]
        [BoxGroup("Generation")]
        public virtual TObj Value { get; set; }

        protected virtual bool SupportsDefaultGeneration { get; set; } = true;

        protected virtual bool IsTemporary => false;

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

        [Button("Generate", ButtonSizes.Medium)]
        [HorizontalGroup("Generation/Actions")]
        [NoGUIColor]
        public virtual void Generate()
        {
            Value = default(TObj);
        }

        [Button("Remove", ButtonSizes.Medium)]
        [HorizontalGroup("Generation/Actions")]
        [NoGUIColor]
        public virtual void Remove()
        {
            Value = default(TObj);
        }

        [EnableIf(nameof(IsSavingEnabled))]
        [Button("Save", ButtonSizes.Medium)]
        [HorizontalGroup("Generation/Actions")]
        [NoGUIColor]
        public virtual void Save()
        {
        }
    }
}