using GuldeLib.Extensions;
using GuldeLib.Players;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ActionFactory : Factory<Action, ActionComponent>
    {
        public ActionFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override ActionComponent Create(Action action)
        {
            Component.Points = action.Points;
            Component.PointsPerRound = action.PointsPerRound;

            if (Locator.Time) Locator.Time.YearTicked += Component.OnYearTicked;

            return Component;
        }
    }
}