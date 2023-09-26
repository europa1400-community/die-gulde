using Newtonsoft.Json;

namespace Gulde.Client.Model.Scenes
{
    public class CityElement
    {
        [JsonProperty("height_data1")]
        public int[] HeightData1 { get; set; }
        
        [JsonProperty("num1")]
        public int Num1 { get; set; }
        
        [JsonProperty("num2")]
        public int Num2 { get; set; }
        
        [JsonProperty("flag1")]
        public bool Flag1 { get; set; }
        
        [JsonProperty("height_data2")]
        public int[] HeightData2 { get; set; }
        
        [JsonProperty("has_water_element")]
        public bool HasWaterElement { get; set; }
    }
}