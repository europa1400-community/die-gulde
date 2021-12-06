using GuldeLib.Generators;
using GuldeLib.Names;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Entities
{
    [CreateAssetMenu(menuName = "Entities/Entity")]
    public class Entity : TypeObject<Entity>
    {
        [Optional]
        [Generatable]
        [OdinSerialize]
        public GeneratableNaming Naming { get; set; }
    }
}