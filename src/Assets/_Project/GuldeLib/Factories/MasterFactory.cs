using GuldeLib.Companies;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class MasterFactory : Factory<Master>
    {
        public MasterFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Master master)
        {
            var masterComponent = GameObject.AddComponent<MasterComponent>();

            masterComponent.Autonomy = master.Autonomy.Value;
            masterComponent.Investivity = master.Investivity.Value;
            masterComponent.Riskiness = master.Riskiness.Value;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}