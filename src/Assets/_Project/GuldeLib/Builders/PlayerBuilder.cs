using GuldeLib.Economy;
using GuldeLib.Players;

namespace GuldeLib.Builders
{
    public class PlayerBuilder : Builder<Player>
    {
        public PlayerBuilder WithWealth(Wealth wealth)
        {
            Object.Wealth = wealth;
            return this;
        }

        public PlayerBuilder WithAction(Action action)
        {
            Object.Action = action;
            return this;
        }
    }
}