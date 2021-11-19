using System;
using Sirenix.OdinInspector;

namespace GuldeLib.Generators
{
    public abstract class GeneratableTypeObject<TObj> : Generatable<TObj> where TObj : TypeObject<TObj>
    {
        
    }
}