using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gulde.Client.Model.Scenes
{
    internal class SceneElementGroup
    {
        [JsonProperty("first_element")]
        public SceneElement FirstElement { get; set; }
        
        [JsonProperty("elements")]
        public List<SceneElement> Elements { get; set; }
    }
}
