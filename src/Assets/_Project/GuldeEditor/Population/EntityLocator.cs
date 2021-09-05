using GuldeLib.Entities;
using GuldeLib.Maps;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;

namespace GuldeEditor.Population
{
    public class EntityLocator : OdinEditorWindow
    {
        [MenuItem("Gulde/Entity Locator")]
        static void ShowWindow() => GetWindow<EntityLocator>();

        [OdinSerialize]
        [HorizontalGroup("Locator")]
        [BoxGroup("Locator/Entity")]
        [OnValueChanged("OnEntityChanged")]
        [ValueDropdown("@FindObjectsOfType<EntityComponent>()")]
        [LabelWidth(50)]
        [PropertySpace(10)]
        [InlineEditor(Expanded = true)]
        EntityComponent Entity { get; set; }

        [OdinSerialize]
        [BoxGroup("Locator/Location")]
        [OnValueChanged("OnMapChanged")]
        [ValueDropdown("@FindObjectsOfType<MapComponent>()")]
        [LabelWidth(50)]
        [PropertySpace(10)]
        [InlineEditor(Expanded = true)]
        MapComponent Map { get; set; }

        [OdinSerialize]
        [BoxGroup("Locator/Location")]
        [OnValueChanged("OnLocationChanged")]
        [ValueDropdown("@FindObjectsOfType<LocationComponent>()")]
        [LabelWidth(50)]
        [PropertySpace(10)]
        [InlineEditor(Expanded = true)]
        LocationComponent Location { get; set; }

        [Button]
        [BoxGroup("Locator/Location")]
        [PropertySpace(10)]
        void Register()
        {
            if (!Entity) return;
            if (!(Map || Location)) return;

            if (Map) Map.EntityRegistry.Register(Entity);
            if (Location) Location.EntityRegistry.Register(Entity);
        }

        [Button]
        [BoxGroup("Locator/Location")]
        [PropertySpace(10)]
        void Unregister()
        {
            if (!Entity) return;
            if (!(Map || Location)) return;

            if (Map) Map.EntityRegistry.Unregister(Entity);
            if (Location) Location.EntityRegistry.Unregister(Entity);
        }

        void OnEntityChanged()
        {

        }

        void OnMapChanged()
        {

        }

        void OnLocationChanged()
        {

        }
    }
}