using GuldeLib.Extensions;
using GuldeLib.Players;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ActionFactory : Factory<Action, ActionPointComponent>
    {
        public ActionFactory(Action action, GameObject gameObject = null, GameObject parentObject = null) : base(action, gameObject, parentObject)
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