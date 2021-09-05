using System.Collections.Generic;
using System.Linq;
using GuldeLib.Company.Employees;
using GuldeLib.Production;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;

namespace GuldeEditor.Production
{
    public class Producer : OdinEditorWindow
    {
        [MenuItem("Gulde/Producer")]
        static void ShowWindow() => GetWindow<Producer>();

        [OdinSerialize]
        [ValueDropdown("@FindObjectsOfType<EmployeeComponent>()")]
        EmployeeComponent Employee { get; set; }

        [OdinSerialize]
        [ValueDropdown("@FindObjectsOfType<ProductionComponent>()")]
        ProductionComponent Production { get; set; }

        [ValueDropdown("Recipes")]
        [OdinSerialize]
        Recipe Recipe { get; set; }

        List<Recipe> Recipes => Production ? Production.Registry.Recipes.ToList() : null;

        [Button]
        void Assign()
        {
            if (!Employee) return;
            if (!Production) return;
            if (!Recipe) return;

            Production.Assignment.Assign(Employee, Recipe);
        }

        [Button]
        void Unassign()
        {
            if (!Employee) return;
            if (!Production) return;

            Production.Assignment.Unassign(Employee);
        }
    }
}