using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Pathfinding/Nav")]
    public class Nav : TypeObject<Nav>
    {
        public override bool SupportsNaming => false;
    }
}