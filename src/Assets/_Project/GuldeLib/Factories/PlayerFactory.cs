using GuldeLib.Players;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class PlayerFactory : Factory<Player, PlayerComponent>
    {
        public PlayerFactory(Player player, GameObject gameObject, GameObject parentObject) : base(player, gameObject, parentObject)
        {
        }

        public override PlayerComponent Create()
        {
            var actionFactory = new ActionFactory(TypeObject.Action.Value, GameObject);
            actionFactory.Create();

            var wealthFactory = new WealthFactory(TypeObject.Wealth.Value, GameObject);
            wealthFactory.Create();

            return Component;
        }
    }
}