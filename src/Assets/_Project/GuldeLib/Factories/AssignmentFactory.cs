using GuldeLib.Producing;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class AssignmentFactory : Factory<Assignment, AssignmentComponent>
    {
        public AssignmentFactory(Assignment assignment, GameObject gameObject = null, GameObject parentObject = null) : base(assignment, gameObject, parentObject)
        {
        }

        public override AssignmentComponent Create() => Component;
    }
}