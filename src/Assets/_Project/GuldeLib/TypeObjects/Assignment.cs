using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Producing/Assignment")]
    public class Assignment : TypeObject<Assignment>
    {
        public override bool SupportsNaming => false;
    }
}