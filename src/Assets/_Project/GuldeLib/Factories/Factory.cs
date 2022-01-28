using GuldeLib.Extensions;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Factories
{
    public abstract class Factory<TObj, TCom> where TObj : TypeObject<TObj> where TCom : MonoBehaviour
    {
        protected TObj TypeObject { get; }
        protected GameObject GameObject { get; }
        protected GameObject ParentObject { get; }
        protected TCom Component { get; }

        public Factory(TObj typeObject, GameObject gameObject, GameObject parentObject, bool allowMultiple = false)
        {
            TypeObject = typeObject;
            GameObject = gameObject ? gameObject : new GameObject();
            ParentObject = parentObject;
            if (parentObject) GameObject.transform.SetParent(parentObject.transform);

            if (allowMultiple) Component = GameObject.AddComponent<TCom>();
            else Component = GameObject.GetOrAddComponent<TCom>();
            Component.SetLogLevel(TypeObject.LogLevel);
        }

        public abstract TCom Create();
    }

    public abstract class Factory<TObj> where TObj : TypeObject<TObj>
    {
        protected TObj TypeObject { get; }
        protected GameObject GameObject { get; }
        protected GameObject ParentObject { get; }

        public Factory(TObj typeObject, GameObject gameObject, GameObject parentObject, bool startActive = true)
        {
            TypeObject = typeObject;
            GameObject = gameObject ? gameObject : new GameObject();
            ParentObject = parentObject;
            if (parentObject) GameObject.transform.SetParent(parentObject.transform);
            GameObject.SetActive(startActive);
        }

        public abstract GameObject Create();
    }
}