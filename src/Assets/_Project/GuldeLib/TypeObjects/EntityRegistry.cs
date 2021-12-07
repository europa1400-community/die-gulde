using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Entities/EntityRegistry")]
    public class EntityRegistry : TypeObject<EntityRegistry>
    {
        public override bool SupportsNaming => false;
    }
}