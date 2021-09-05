using GuldeLib.Entities;
using GuldeLib.Maps;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;

namespace GuldeEditor.Population
{
    public class EntityTraveler : OdinEditorWindow
    {
        [MenuItem("Gulde/Entity Traveler")]
        static void ShowWindow() => GetWindow<EntityTraveler>();

        [OdinSerialize]
        [ValueDropdown("@FindObjectsOfType<TravelComponent>()")]
        TravelComponent Traveller { get; set; }

        [OdinSerialize]
        [ValueDropdown("@FindObjectsOfType<LocationComponent>()")]
        LocationComponent Location { get; set; }

        [Button]
        void Travel()
        {
            if (!Traveller) return;
            if (!Location) return;

            Traveller.TravelTo(Location);
        }
    }
}