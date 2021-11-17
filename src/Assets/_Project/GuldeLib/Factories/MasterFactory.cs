using GuldeLib.Companies;
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

            masterComponent.Autonomy = master.Autonomy;
            masterComponent.Investivity = master.Investivity;
            masterComponent.Riskiness = master.Riskiness;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}