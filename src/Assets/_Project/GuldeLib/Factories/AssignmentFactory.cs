using GuldeLib.Producing;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class AssignmentFactory : Factory<Assignment>
    {
        public AssignmentFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Assignment assignment)
        {
            var assignmentComponent = GameObject.AddComponent<AssignmentComponent>();

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}