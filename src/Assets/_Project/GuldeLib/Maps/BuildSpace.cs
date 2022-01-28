using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Maps
{
    [Serializable]
    public class BuildSpace
    {
        [OdinSerialize]
        public Vector2Int Size { get; set; }

        [OdinSerialize]
        public int Count { get; set; }

        [OdinSerialize]
        public BuildSpaceType Type { get; set; }
    }
}