using CarSimulator.Analysis.Services;
using Microsoft.Extensions.Configuration;

// Läs analysprojektets inställningar från User Secrets.
var configuration = new ConfigurationBuilder ()
    .AddUserSecrets<Program> ()
    .Build ();

var channelId = configuration["ThingSpeak:ChannelId"];
var readApiKey = configuration["ThingSpeak:ReadApiKey"];

// Kontrollera inställningarna innan vi anropar ThingSpeak.
if (!long.TryParse (channelId, out var id) || id <= 0 ||
    string.IsNullOrWhiteSpace (readApiKey))
    {
    Console.WriteLine ("Ange ett giltigt kanal-ID och en läsnyckel i User Secrets.");
    Console.WriteLine ("Tryck på valfri tangent för att avsluta.");
    Console.ReadKey ();
    return;
    }

// Återanvänd HTTP-klienten under programmets körning.
using var httpClient = new HttpClient
    {
    Timeout = TimeSpan.FromSeconds (15)
    };

var thingSpeakService = new ThingSpeakService (
    httpClient, channelId!, readApiKey);

// Testa att hämta de senaste 100 avläsningarna.
Console.WriteLine ("Hämtar data från ThingSpeak...");
var feeds = await thingSpeakService.GetLast100Async ();

if (feeds is not null)
    {
    Console.WriteLine ($"Antal hämtade avläsningar: {feeds.Count}");
    }

Console.WriteLine ("Tryck på valfri tangent för att avsluta.");
Console.ReadKey ();