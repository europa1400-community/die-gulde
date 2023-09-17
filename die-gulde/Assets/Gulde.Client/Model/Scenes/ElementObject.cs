using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Gulde.Client.Model.Scenes
{
    internal class ElementObject
    {
        public string Name { get; set; }

        [JsonProperty("offset")]
        public Vector3 PositionOffset { get; set; }

        [JsonProperty("rotation")]
        public Vector3 RotationOffset { get; set; }
    }
}
