using System.Globalization;
using carsimulator.Models;

namespace carsimulator.Services;

public class ThingSpeakService
    {
    private readonly HttpClient _httpClient;
    private readonly string _writeApiKey;

    public ThingSpeakService ( HttpClient httpClient, string writeApiKey )
        {
        _httpClient = httpClient;
        _writeApiKey = writeApiKey;
        }

    public async Task SendTelemetryAsync ( CarTelemetry data )
        {
        // Koppla bilens värden till rätt fält i ThingSpeak.
        
        var payload = new Dictionary<string, string>
            {
            ["api_key"] = _writeApiKey,
            ["field1"] = data.Rpm.ToString (CultureInfo.InvariantCulture),
            ["field2"] = data.Speed.ToString (CultureInfo.InvariantCulture),
            ["field3"] = data.EngineTemp.ToString (CultureInfo.InvariantCulture),
            ["field4"] = data.FuelLevel.ToString (CultureInfo.InvariantCulture),
            ["created_at"] = data.Timestamp.ToString (
                "o", CultureInfo.InvariantCulture) // InvariantCulture ger decimalpunkt oavsett datorns språk.
            };

        try
            {
            // Skicka avläsningen med HTTPS.
            using var content = new FormUrlEncodedContent (payload);

            using var response = await _httpClient.PostAsync (
                "https://api.thingspeak.com/update",
                content);

            if (!response.IsSuccessStatusCode)
                {
                Console.WriteLine (
                    $"Kunde inte skicka data. HTTP-status: {(int) response.StatusCode}");
                return;
                }

            // Kontrollera att ThingSpeak faktiskt sparade avläsningen.
            var result = await response.Content.ReadAsStringAsync ();

            if (long.TryParse (result.Trim (), out var entryId) && entryId > 0)
                {
                Console.WriteLine ($"Data sparad i ThingSpeak. ID: {entryId}");
                }
            else
                {
                Console.WriteLine ("ThingSpeak kunde inte spara avläsningen.");
                }
            }
        catch (HttpRequestException)
            {
            // Ett anslutningsfel ska inte krascha simuleringen.
            Console.WriteLine ("Anslutningsfel: kunde inte nå ThingSpeak.");
            }
        catch (TaskCanceledException)
            {
            // Hantera att servern inte svarar inom tidsgränsen.
            Console.WriteLine ("Tidsgränsen för anropet till ThingSpeak överskreds.");
            }
        }
    }