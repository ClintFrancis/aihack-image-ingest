using System;
using Newtonsoft.Json;

public class MetaData
{
    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("processType")]
    public string ProcessType { get; set; }

    [JsonProperty("sourceDateTime")]
    public DateTime SourceDateTime { get; set; }

    [JsonProperty("isDaylight")]
    public bool IsDaylight { get; set; }

    [JsonProperty("sourceTags")]
    public string[] Tags { get; set; }
}