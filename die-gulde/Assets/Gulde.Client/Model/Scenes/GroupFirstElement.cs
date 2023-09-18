using Newtonsoft.Json;

namespace Assets.Gulde.Client.Model.Scenes
{
    internal class GroupFirstElement
    {
        public string Name { get; set; }

        [JsonProperty("dummy_element")]
        public DummyElement DummyElement { get; set; }

        [JsonProperty("object_element")]
        public ElementObject Element { get; set; }
    }
}