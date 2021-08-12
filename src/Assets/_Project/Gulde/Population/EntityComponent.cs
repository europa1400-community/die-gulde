using System.Collections;
using System.Collections.Generic;
using Gulde.Buildings;
using Gulde.Extensions;
using Gulde.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Population
{
    public class EntityComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public LocationComponent Location { get; set; }

        [OdinSerialize]
        public MapComponent Map { get; set; }
    }
}