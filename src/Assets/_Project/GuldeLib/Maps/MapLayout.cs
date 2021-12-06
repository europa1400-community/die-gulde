using System;
using System.Collections.Generic;
using GuldeLib.Generators;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Maps
{
    [Serializable]
    public class MapLayout
    {
        [OdinSerialize]
        public Dictionary<Vector2Int, BuildSpace> CellToBuildSpace { get; set; } = new Dictionary<Vector2Int, BuildSpace>();
    }
}