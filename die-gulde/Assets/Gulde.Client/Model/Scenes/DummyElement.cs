using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Gulde.Client.Model.Scenes
{
    internal class DummyElement
    {
        [JsonProperty("offset")]
        public Vector3 Position { get; set; }

        [JsonProperty("rotation")]
        public Vector3 Rotation { get; set; }
    }
}
