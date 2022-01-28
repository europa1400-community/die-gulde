using GuldeLib.Companies;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class MasterBuilder : Builder<Master>
    {
        public MasterBuilder WithRiskiness(float riskiness)
        {
            Object.Riskiness = riskiness;
            return this;
        }

        public MasterBuilder WithInvestivity(float investivity)
        {
            Object.Investivity = investivity;
            return this;
        }

        public MasterBuilder WithAutonomy(float autonomy)
        {
            Object.Autonomy = autonomy;
            return this;
        }
    }
}