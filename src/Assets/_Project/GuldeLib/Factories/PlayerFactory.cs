using GuldeLib.Players;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class PlayerFactory : Factory<Player, PlayerComponent>
    {
        public PlayerFactory(GameObject gameObject, GameObject parentObject) : base(gameObject, parentObject)
        {
        }

        public override PlayerComponent Create(Player player)
        {
            var actionFactory = new ActionFactory(GameObject);
            actionFactory.Create(player.Action.Value);

            var wealthFactory = new WealthFactory(GameObject);
            wealthFactory.Create(player.Wealth.Value);

            return Component;
        }
    }
}