using Newtonsoft.Json;

namespace Assets.Gulde.Client.Model.Scenes
{
    internal class SceneElementGroup
    {
        public SceneElement[] Elements { get; set; }

        [JsonProperty("first_element")]
        public GroupFirstElement FirstElement { get; set; }
    }
}
