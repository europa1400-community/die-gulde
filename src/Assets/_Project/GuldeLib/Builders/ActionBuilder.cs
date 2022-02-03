using GuldeLib.Players;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class ActionBuilder : Builder<ActionPoint>
    {
        public ActionBuilder WithPointsPerRound(int pointsPerRound)
        {
            Object.PointsPerRound = pointsPerRound;
            return this;
        }

        public ActionBuilder WithPoints(int points)
        {
            Object.Points = points;
            return this;
        }
    }
}