using System;
using Newtonsoft.Json;

public class Tag {
    // JSON properties
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }
}