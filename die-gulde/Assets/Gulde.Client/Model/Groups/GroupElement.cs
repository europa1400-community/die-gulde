using Gulde.Client.Model.Common;
using Newtonsoft.Json;

namespace Gulde.Client.Model.Groups
{
    internal class GroupElement
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public GroupElementType Type { get; set; }

        [JsonProperty("object_name")]
        public string ObjectName { get; set; }

        [JsonProperty("transform")]
        public ElementTransform Transform { get; set; }

        [JsonProperty("additional_transform")]
        public ElementTransform AdditionalTransform { get; set; }
    }
}