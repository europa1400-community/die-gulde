using UnityEngine;

namespace GuldeLib.Extensions
{
    public static class GameObjectExtensions
    {
        public static TCom GetOrAddComponent<TCom>(this GameObject gameObject) where TCom : MonoBehaviour
        {
            var component = gameObject.GetComponent<TCom>();
            if (!component) component = gameObject.AddComponent<TCom>();
            return component;
        }
    }
}