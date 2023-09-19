using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Gulde.Client.Model.Scenes
{
    internal class ObjectElement
    {
        public string Name { get; set; }

        [JsonProperty("transform")]
        public ElementTransform Transform { get; set; }

        [JsonProperty("transforms")]
        public List<ElementTransform> Transforms { get; set; }
    }
}
