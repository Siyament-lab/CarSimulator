using CarSimulator.Analysis.Menu;
using CarSimulator.Analysis.Services;
using Microsoft.Extensions.Configuration;

// Läs analysprojektets inställningar från User Secrets.
var configuration = new ConfigurationBuilder ()
    .AddUserSecrets<Program> ()
    .Build ();

var channelId = configuration["ThingSpeak:ChannelId"];
var readApiKey = configuration["ThingSpeak:ReadApiKey"];

// Kontrollera inställningarna innan programmet startar.
if (!long.TryParse (channelId, out var id) || id <= 0 ||
    string.IsNullOrWhiteSpace (readApiKey))
    {
    Console.WriteLine ("Ange ett giltigt kanal-ID och en läsnyckel i User Secrets.");
    Console.WriteLine ("Tryck på valfri tangent för att avsluta.");
    Console.ReadKey ();
    return;
    }

// Återanvänd samma HTTP-klient under hela körningen.
using var httpClient = new HttpClient
    {
    Timeout = TimeSpan.FromSeconds (15)
    };

// Skapa tjänsterna och koppla dem till menyn.
var thingSpeakService = new ThingSpeakService (
    httpClient, channelId!, readApiKey);

var analysisService = new TelemetryAnalysisService ();

var menu = new AnalysisMenu (thingSpeakService, analysisService);

// Visa menyn tills användaren väljer att avsluta.
await menu.ShowAsync ();