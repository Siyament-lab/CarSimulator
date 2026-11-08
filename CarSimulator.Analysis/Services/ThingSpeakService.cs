using System.Text.Json;
using CarSimulator.Analysis.DTOs;

namespace CarSimulator.Analysis.Services;

public class ThingSpeakService
    {
    private readonly HttpClient _httpClient;
    private readonly string _channelId;
    private readonly string _readApiKey;

    public ThingSpeakService (
        HttpClient httpClient,
        string channelId,
        string readApiKey )
        {
        _httpClient = httpClient;
        _channelId = channelId;
        _readApiKey = readApiKey;
        }

    // Hämta avläsningar från de senaste 24 timmarna.
    public async Task<List<ThingSpeakFeedDto>?> GetLast24HoursAsync ()
        {
        return await GetFeedsAsync ("minutes=1440&results=8000");
        }

    // Hämta de senaste 100 avläsningarna.
    public async Task<List<ThingSpeakFeedDto>?> GetLast100Async ()
        {
        return await GetFeedsAsync ("results=100");
        }

    // Gemensam metod för anrop och felhantering.
    private async Task<List<ThingSpeakFeedDto>?> GetFeedsAsync ( string query )
        {
        var url =
            $"https://api.thingspeak.com/channels/{Uri.EscapeDataString (_channelId)}" +
            $"/feeds.json?api_key={Uri.EscapeDataString (_readApiKey)}&{query}";

        try
            {
            using var response = await _httpClient.GetAsync (url);

            if (!response.IsSuccessStatusCode)
                {
                Console.WriteLine (
                    $"Kunde inte hämta data. HTTP-status: {(int) response.StatusCode}");
                return null;
                }

            var json = await response.Content.ReadAsStringAsync ();

            // ThingSpeak kan returnera -1 om åtkomst saknas.
            if (json.Trim () == "-1")
                {
                Console.WriteLine ("Åtkomst nekad. Kontrollera kanal-ID och läsnyckel.");
                return null;
                }

            // Omvandla JSON-svaret till våra DTO-klasser.
            var result = JsonSerializer.Deserialize<ThingSpeakResponseDto> (json);

            if (result?.Feeds is null)
                {
                Console.WriteLine ("Svaret från ThingSpeak saknar avläsningar.");
                return null;
                }

            return result.Feeds;
            }
        catch (HttpRequestException)
            {
            Console.WriteLine ("Anslutningsfel: kunde inte nå ThingSpeak.");
            }
        catch (TaskCanceledException)
            {
            Console.WriteLine ("Tidsgränsen för anropet till ThingSpeak överskreds.");
            }
        catch (JsonException)
            {
            Console.WriteLine ("Svaret från ThingSpeak kunde inte tolkas.");
            }

        // Null betyder att hämtningen misslyckades.
        return null;
        }
    }