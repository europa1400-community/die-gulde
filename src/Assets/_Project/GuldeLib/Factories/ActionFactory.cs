using GuldeLib.Players;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ActionFactory : Factory<Action>
    {
        public ActionFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Action action)
        {
            var actionComponent = GameObject.AddComponent<ActionComponent>();

            actionComponent.Points = action.Points;
            actionComponent.PointsPerRound = action.PointsPerRound;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}