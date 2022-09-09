using Newtonsoft.Json;

public class MergedData:Fauna {

    public MergedData(Fauna fauna, Weather weather)
    {
        this.Id = fauna.Id;
        this.Domain = fauna.Domain;
        this.SubDomain = fauna.SubDomain;
        this.SpeciesOrder = fauna.SpeciesOrder;
        this.CommonName = fauna.CommonName;
        this.ScientificName = fauna.ScientificName;
        this.Location = fauna.Location;
        this.Confidence = fauna.Confidence;
        this.Tags = fauna.Tags;
        this.Dataset = fauna.Dataset;
        this.Media = fauna.Media;
        this.Metadata = fauna.Metadata;
        this.Weather = weather;
    }

    [JsonProperty("weather")]
    public Weather Weather { get; set; } = new Weather();
}