using System.Text.Json.Serialization;

namespace CarSimulator.Analysis.DTOs;

public class ThingSpeakResponseDto
    {
    // Listan med avläsningar som hämtas från ThingSpeak.
    [JsonPropertyName ("feeds")]
    public List<ThingSpeakFeedDto> Feeds { get; set; } = new ();
    }