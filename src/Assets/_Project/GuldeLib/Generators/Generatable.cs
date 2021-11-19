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
        protected bool IsGenerated { get; set; }

        public abstract void Generate();

        public void Remove()
        {
            Value = default(TObj);
        }
    }
}