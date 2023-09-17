using Assets.Gulde.Client.Model.Enum;
using Newtonsoft.Json;

namespace Assets.Gulde.Client.Model
{
    internal class GroupElement
    {
        public string Name { get; set; }

        public ElementType Type { get; set; }

        [JsonProperty("object_name")]
        public string ObjectName { get; set; }

        public ElementTransform Transform { get; set; }

        [JsonProperty("additional_transform")]
        public ElementTransform AdditionalTransform { get; set; }
    }
}