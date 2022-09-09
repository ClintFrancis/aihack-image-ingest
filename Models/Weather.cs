using System;
using Newtonsoft.Json;

public class Weather {
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [JsonProperty("datetime")]
    public DateTime DateTime { get; set; }

    [JsonProperty("temperature_celsius")]
    public double TemperatureCelsius { get; set; }

    [JsonProperty("dew_point_celsius")]
    public double DewPointCelsius { get; set; }

    [JsonProperty("humidity_percentage")]
    public double HumidityPercentage { get; set; }

    [JsonProperty("wind")]
    public string Wind { get; set; }

    [JsonProperty("speed_mph")]
    public double SpeedMph { get; set; }

    [JsonProperty("gust_mph")]
    public double GustMph { get; set; }

    [JsonProperty("pressure")]
    public double Pressure { get; set; }

    [JsonProperty("precip_rate")]
    public double PrecipRate { get; set; }

    [JsonProperty("precip_accum")]
    public double PrecipAccum { get; set; }

    [JsonProperty("uv")]
    public double Uv { get; set; }

    [JsonProperty("solar")]
    public double Solar { get; set; }

    [JsonProperty("station")]
    public string Station { get; set; }

    // To String all the fields
    public override string ToString() {
        return $"Id: {Id}, DateTime: {DateTime}, TemperatureCelsius: {TemperatureCelsius}, DewPointCelsius: {DewPointCelsius}, HumidityPercentage: {HumidityPercentage}, Wind: {Wind}, SpeedMph: {SpeedMph}, GustMph: {GustMph}, Pressure: {Pressure}, PrecipRate: {PrecipRate}, PrecipAccum: {PrecipAccum}, Uv: {Uv}, Solar: {Solar}, Station: {Station}";
    }
}