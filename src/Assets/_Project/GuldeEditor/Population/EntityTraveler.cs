using Gulde.Entities;
using Gulde.Maps;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;

namespace GuldeEditor.Population
{
    public class EntityTraveler : OdinEditorWindow
    {
        [MenuItem("Gulde/EntityTraveler")]
        static void ShowWindow() => GetWindow<EntityTraveler>();

        [OdinSerialize]
        TravelComponent Traveller { get; set; }

        [OdinSerialize]
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