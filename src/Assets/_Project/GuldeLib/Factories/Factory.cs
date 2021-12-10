using GuldeLib.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Factories
{
    public abstract class Factory<TObj, TCom> where TObj : SerializedScriptableObject where TCom : MonoBehaviour
    {
        protected GameObject GameObject { get; }
        protected GameObject ParentObject { get; }
        protected TCom Component { get; }

        public Factory(GameObject gameObject, GameObject parentObject, bool allowMultiple = false)
        {
            GameObject = gameObject ? gameObject : new GameObject();
            ParentObject = parentObject;
            if (parentObject) GameObject.transform.SetParent(parentObject.transform);

            if (allowMultiple) Component = GameObject.AddComponent<TCom>();
            else Component = GameObject.GetOrAddComponent<TCom>();
        }

        public abstract TCom Create(TObj obj);
    }

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

        public abstract GameObject Create(TObj obj);
    }
}