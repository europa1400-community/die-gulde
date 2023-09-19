using Assets.Gulde.Client.Model.Scenes;
using Newtonsoft.Json;

namespace Assets.Gulde.Client.Model
{
    internal class GildeScene
    {
        [JsonProperty("element_groups")]
        public SceneElementGroup[] ElementGroups { get; set; }
    }
}
