using System;
using System.Collections.Generic;
using System.Linq;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Persons/Person")]
    public class Person : TypeObject<Person>
    {
        public override bool SupportsNaming => true;
    }
}