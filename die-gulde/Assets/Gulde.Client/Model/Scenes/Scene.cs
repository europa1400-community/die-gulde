using Assets.Gulde.Client.Model.Scenes;
using Newtonsoft.Json;

namespace Assets.Gulde.Client.Model
{
    internal class Scene
    {
        public string Path { get; set; }

        public bool? Skipped { get; set; }

        [JsonProperty("element_groups")]
        public SceneElementGroup[] ElementGroups { get; set; }
    }
}
