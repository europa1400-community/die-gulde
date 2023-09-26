using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gulde.Client.Model.Groups
{
    internal class GildeGroup
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("elements")]
        public List<GroupElement> Elements { get; set; }
    }
}
