using Newtonsoft.Json;

public class Location {

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("region")]
    public string Region { get; set; }

    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }

    [JsonProperty("globalID")]
    public string GlobalId { get; set; }
    
    [JsonProperty("pointX")]
    public double PointX { get; set; }

    [JsonProperty("pointY")]
    public double PointY { get; set; }
}