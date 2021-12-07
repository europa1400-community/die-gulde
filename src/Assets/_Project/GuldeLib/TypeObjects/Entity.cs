using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Entities/Entity")]
    public class Entity : TypeObject<Entity>
    {
        public override bool SupportsNaming => true;
    }
}