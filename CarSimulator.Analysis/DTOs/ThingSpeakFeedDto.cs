using System.Text.Json.Serialization;

namespace CarSimulator.Analysis.DTOs;

public class ThingSpeakFeedDto
    {
    // Tidpunkten då avläsningen registrerades.
    [JsonPropertyName ("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    // Avläsningens ID i ThingSpeak.
    [JsonPropertyName ("entry_id")]
    public long EntryId { get; set; }

    // ThingSpeak skickar fältvärden som text.
    [JsonPropertyName ("field1")]
    public string? Rpm { get; set; }

    [JsonPropertyName ("field2")]
    public string? Speed { get; set; }

    [JsonPropertyName ("field3")]
    public string? EngineTemp { get; set; }

    [JsonPropertyName ("field4")]
    public string? FuelLevel { get; set; }
    }