using System.Collections.Generic;
using GuldeLib.Economy;
using UnityEngine;

namespace GuldeLib.Economy
{
    public class Exchange : ScriptableObject
    {
        public Dictionary<Item, int> StartItems { get; set; } = new Dictionary<Item, int>();
        public int Slots { get; set; } = int.MaxValue;
        public bool IsAccepting { get; set; } = true;
    }
}