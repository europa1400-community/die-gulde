using GuldeLib.Producing;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class AssignmentFactory : Factory<Assignment, AssignmentComponent>
    {
        public AssignmentFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override AssignmentComponent Create(Assignment assignment) => Component;
    }
}