using Newtonsoft.Json;

namespace Gulde.Client.Model.Scenes
{
    internal class GildeScene
    {
        [JsonProperty("element_groups")]
        public SceneElementGroup[] ElementGroups { get; set; }
    }
}
