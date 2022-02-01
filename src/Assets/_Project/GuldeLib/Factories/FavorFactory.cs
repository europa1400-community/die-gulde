using GuldeLib.Society;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class FavorFactory : Factory<Favor, FavorComponent>
    {
        public FavorFactory(Favor social, GameObject gameObject, bool allowMultiple = false) : base(social, gameObject, null, allowMultiple)
        {
        }

        public override FavorComponent Create()
        {
            return Component;
        }
    }
}