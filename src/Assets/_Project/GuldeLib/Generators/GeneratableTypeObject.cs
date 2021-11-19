using Sirenix.OdinInspector;

namespace GuldeLib.Generators
{
    public abstract class GeneratableTypeObject<TObj> : Generatable<TObj> where TObj : TypeObject<TObj>
    {
        [BoxGroup("Generation")]
        public override TObj Value { get; set; }
    }
}