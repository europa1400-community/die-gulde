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
            var productionAgentFactory = new ProductionAgentFactory(TypeObject.ProductionAgent.Value, GameObject);
            productionAgentFactory.Create();

            return Component;
        }
    }
}