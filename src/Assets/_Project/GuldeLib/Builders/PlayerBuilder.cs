using GuldeLib.Economy;
using GuldeLib.Generators;
using GuldeLib.Players;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class PlayerBuilder : Builder<Citizen>
    {
        public PlayerBuilder WithWealth(GeneratableWealth wealth)
        {
            Object.Wealth = wealth;
            return this;
        }

        public PlayerBuilder WithAction(GeneratableActionPoint actionPoint)
        {
            Object.ActionPoint = actionPoint;
            return this;
        }
    }
}