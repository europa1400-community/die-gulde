using GuldeLib.Society;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class TalentFactory : Factory<Talent, TalentComponent>
    {
        public TalentFactory(Talent typeObject, GameObject gameObject, bool allowMultiple = false) : base(typeObject, gameObject, null, allowMultiple)
        {
        }

        public override TalentComponent Create()
        {
            foreach (var pair in TypeObject.TalentToPoints)
            {
                Component.TalentToPoints[pair.Key] = pair.Value;
            }

            return Component;
        }
    }
}