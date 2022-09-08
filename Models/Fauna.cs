using System.Collections.Generic;
using Newtonsoft.Json;

public class Fauna {
    [JsonProperty("speciesOrder")]
    public string SpeciesOrder { get; set; }

    [JsonProperty("commonName")]
    public string CommonName { get; set; }

    [JsonProperty("scientificName")]
    public string ScientificName { get; set; }

    [JsonProperty("isNative")]
    public bool IsNative { get; set; }

    [JsonProperty("isEndangered")]
    public bool IsEndangered { get; set; }

    [JsonProperty("isInvasive")]
    public bool IsInvasive { get; set; }

    [JsonProperty("isThreatened")]
    public bool IsThreatened { get; set; }

    [JsonProperty("isProtected")]
    public bool IsProtected { get; set; }

    [JsonProperty("isPredator")]
    public bool IsPredator { get; set; }

    [JsonProperty("location")]
    public Location Location { get; set; } = new Location();

    [JsonProperty("confidence")]
    public double Confidence { get; set; }

    [JsonProperty("tags")]
    public List<Tag> Tags { get; set; } = new List<Tag>();

    [JsonProperty("dataset")]
    public string Dataset { get; set; }

    [JsonProperty("Media")]
    public Media Media { get; set; } = new Media();

    [JsonProperty("_metadata")]
    public MetaData Metadata { get; set; } = new MetaData();

    // ToString
    public override string ToString() {
        return $"{CommonName} ({ScientificName})";
    }
}