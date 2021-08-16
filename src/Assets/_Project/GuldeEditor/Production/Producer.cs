using System.Collections.Generic;
using System.Linq;
using Gulde.Company;
using Gulde.Production;
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
        EmployeeComponent Employee { get; set; }

        [OdinSerialize]
        ProductionComponent Production { get; set; }

        [ValueDropdown("Recipes")]
        [OdinSerialize]
        Recipe Recipe { get; set; }

        List<Recipe> Recipes => Production ? Production.Recipes.ToList() : null;

        [Button]
        void Assign()
        {
            if (!Employee) return;
            if (!Production) return;
            if (!Recipe) return;

            Production.Assign(Employee, Recipe);
        }

        [Button]
        void Unassign()
        {
            if (!Employee) return;
            if (!Production) return;

            Production.Unassign(Employee);
        }
    }
}