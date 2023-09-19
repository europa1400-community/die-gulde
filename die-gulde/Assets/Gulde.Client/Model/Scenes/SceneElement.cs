using Gulde.Client.Model.Scenes;
using Newtonsoft.Json;

namespace Assets.Gulde.Client.Model.Scenes
{
    internal class SceneElement
    {
        public string Name { get; set; }

        [JsonProperty("dummy_element")]
        public DummyElement DummyElement { get; set; }

        [JsonProperty("object_element")]
        public ObjectElement ObjectElement { get; set; }
    }
}
