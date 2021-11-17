using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Builders
{
    public abstract class Builder<TObj> where TObj : SerializedScriptableObject
    {
        protected TObj Object { get; }

        public Builder()
        {
            Object = ScriptableObject.CreateInstance<TObj>();
        }

        public TObj Build() => Object;
    }
}