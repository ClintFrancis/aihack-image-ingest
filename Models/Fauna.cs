using System.Collections.Generic;
using Newtonsoft.Json;

public class Fauna {
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("domain")]
    public string Domain { get; set; }

    [JsonProperty("subDomain")]
    public string SubDomain { get; set; }

    [JsonProperty("speciesOrder")]
    public string SpeciesOrder { get; set; }

    [JsonProperty("commonName")]
    public string CommonName { get; set; }

    [JsonProperty("scientificName")]
    public string ScientificName { get; set; }

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