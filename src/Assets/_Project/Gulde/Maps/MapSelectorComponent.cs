using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Maps
{
    public class MapSelectorComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [InlineButton("Select")]
        public MapComponent SelectedMap { get; set; }

        void Awake()
        {
            Locator.MapSelectorComponent = this;
        }

        void Select()
        {
            if (!SelectedMap) return;

            foreach (var map in MapRegistry.Maps)
            {
                map.SetVisible(map == SelectedMap);
                Debug.Log($"Set map {map.name} to {(map == SelectedMap ? "visible" : "invisible")}");
            }
        }

        #region OdinInspector

        void OnValidate()
        {
            Locator.MapSelectorComponent = this;
        }

        #endregion
    }
}