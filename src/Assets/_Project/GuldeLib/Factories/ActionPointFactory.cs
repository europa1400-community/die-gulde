using GuldeLib.Extensions;
using GuldeLib.Players;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ActionPointFactory : Factory<ActionPoint, ActionPointComponent>
    {
        public ActionPointFactory(ActionPoint actionPoint, GameObject gameObject = null, GameObject parentObject = null) : base(actionPoint, gameObject, parentObject)
        {
        }

        public override ActionPointComponent Create()
        {
            Component.Points = TypeObject.Points;
            Component.PointsPerRound = TypeObject.PointsPerRound;

            if (Locator.Time) Locator.Time.YearTicked += Component.OnYearTicked;

            return Component;
        }
    }
}