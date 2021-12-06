using GuldeLib.Players;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class PlayerFactory : Factory<Player>
    {
        public PlayerFactory(GameObject gameObject, GameObject parentObject) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Player player)
        {
            var actionFactory = new ActionFactory(GameObject);
            actionFactory.Create(player.Action.Value);

            var wealthFactory = new WealthFactory(GameObject);
            wealthFactory.Create(player.Wealth.Value);

            var playerComponent = GameObject.AddComponent<PlayerComponent>();

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}