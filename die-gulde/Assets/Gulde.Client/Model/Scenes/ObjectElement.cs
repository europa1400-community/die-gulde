using Newtonsoft.Json;

namespace Gulde.Client.Model.Scenes
{
    internal class ObjectElement : TransformElement
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
