using System.Collections.Generic;
using Newtonsoft.Json;

namespace Assets.Gulde.Client.Model.Scenes
{
    internal class SceneElementGroup
    {
        [JsonProperty("first_element")]
        public SceneElement FirstElement { get; set; }
        
        public List<SceneElement> Elements { get; set; }
    }
}
