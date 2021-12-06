using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Entities
{
    [CreateAssetMenu(menuName = "Entities/EntityRegistry")]
    public class EntityRegistry : TypeObject<EntityRegistry>
    {
        public override bool HasNaming => false;
    }
}