using GuldeLib.Companies;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class MasterFactory : Factory<Master, MasterComponent>
    {
        public MasterFactory(Master master, GameObject gameObject = null, GameObject parentObject = null) : base(master, gameObject, parentObject)
        {
        }

        public override MasterComponent Create()
        {
            Component.Autonomy = TypeObject.Autonomy.Value;
            Component.Investivity = TypeObject.Investivity.Value;
            Component.Riskiness = TypeObject.Riskiness.Value;

            var productionAgentFactory = new ProductionAgentFactory(TypeObject.ProductionAgent.Value, GameObject);
            productionAgentFactory.Create();

            return Component;
        }
    }
}