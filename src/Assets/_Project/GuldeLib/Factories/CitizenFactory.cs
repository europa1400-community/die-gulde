using GuldeLib.Players;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class CitizenFactory : Factory<Citizen, CitizenComponent>
    {
        public CitizenFactory(Citizen citizen, GameObject gameObject, GameObject parentObject) : base(citizen, gameObject, parentObject)
        {
        }

        public override CitizenComponent Create()
        {
            var actionPointFactory = new ActionPointFactory(TypeObject.ActionPoint.Value, GameObject);
            actionPointFactory.Create();

            var wealthFactory = new WealthFactory(TypeObject.Wealth.Value, GameObject);
            wealthFactory.Create();

            var favorFactory = new FavorFactory(TypeObject.Favor.Value, GameObject);
            favorFactory.Create();

            var talentFactory = new TalentFactory(TypeObject.Talent.Value, GameObject);
            talentFactory.Create();

            return Component;
        }
    }
}