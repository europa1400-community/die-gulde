using Newtonsoft.Json;

namespace Assets.Gulde.Client.Model.Scenes
{
    internal class SceneElement
    {
        public string Name { get; set; }

        [JsonProperty("object_element")]
        public ElementObject Object { get; set; }
    }
}
