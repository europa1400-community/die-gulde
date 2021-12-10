using GuldeLib.Companies;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class MasterFactory : Factory<Master, MasterComponent>
    {
        public MasterFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override MasterComponent Create(Master master)
        {
            Component.Autonomy = master.Autonomy.Value;
            Component.Investivity = master.Investivity.Value;
            Component.Riskiness = master.Riskiness.Value;

            var productionAgentFactory = new ProductionAgentFactory(GameObject);

            return Component;
        }
    }
}