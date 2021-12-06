using GuldeLib.Economy;
using GuldeLib.Generators;
using GuldeLib.Players;

namespace GuldeLib.Builders
{
    public class PlayerBuilder : Builder<Player>
    {
        public PlayerBuilder WithWealth(GeneratableWealth wealth)
        {
            Object.Wealth = wealth;
            return this;
        }

        public PlayerBuilder WithAction(GeneratableAction action)
        {
            Object.Action = action;
            return this;
        }
    }
}