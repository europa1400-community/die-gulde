using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Gulde.Client.Model.Scenes
{
    internal class ElementTransform
    {
        public string Name { get; set; }

        [JsonProperty("position")]
        public Vector3 PositionOffset { get; set; }

        [JsonProperty("rotation")]
        public Vector3 RotationOffset { get; set; }
    }
}