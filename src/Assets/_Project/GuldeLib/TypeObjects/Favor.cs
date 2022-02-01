using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(fileName = "favor", menuName = "Society/Favor")]
    public class Favor : TypeObject<Favor>
    {
        public override bool SupportsNaming => false;
    }
}