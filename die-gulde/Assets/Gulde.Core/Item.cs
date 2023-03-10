using System.Collections.Generic;
using UnityEngine;

namespace Gulde.Core
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item")]
    public class Item : ScriptableObject
    {
        [SerializeField]
        public string friendlyName;

        [SerializeField]
        public List<ItemFlag> itemFlags = new();

        [SerializeField]
        public int meanPrice;
    }
}