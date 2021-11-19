using System;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Generators
{
    [Serializable]
    public abstract class Generatable<TObj>
    {
        [OdinSerialize]
        [BoxGroup("Generation")]
        [Generatable]
        [DisableIf(nameof(IsGenerated))]
        public virtual TObj Value { get; set; }

        [OdinSerialize]
        [BoxGroup("Generation")]
        [InlineButton(nameof(Remove), "Clear")]
        [InlineButton(nameof(Generate), "Generate")]
        [PropertyOrder(Single.PositiveInfinity)]
        protected bool IsGenerated { get; set; } = true;

        public abstract void Generate();

        public void Remove()
        {
            Value = default(TObj);
        }

        public static implicit operator TObj(Generatable<TObj> generatable) => generatable.Value;
    }
}