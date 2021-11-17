using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Factories
{
    public abstract class Factory<TObj> where TObj : SerializedScriptableObject
    {
        protected GameObject GameObject { get; }
        protected GameObject ParentObject { get; }

        public Factory(GameObject gameObject, GameObject parentObject)
        {
            GameObject = gameObject ? gameObject : new GameObject();
            ParentObject = parentObject;
            if (parentObject) GameObject.transform.SetParent(parentObject.transform);
        }

        public Factory()
        {
        }

        public abstract GameObject Create(TObj obj);

        public abstract GameObject Generate();
    }
}