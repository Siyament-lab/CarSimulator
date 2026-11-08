using carsimulator.Services;
using Microsoft.Extensions.Configuration;

// Läs inställningar från projektets User Secrets.
var configuration = new ConfigurationBuilder ()
    .AddUserSecrets<Program> ()
    .Build ();

// Hämta API-nyckeln utan att skriva ut den.
var writeApiKey = configuration["ThingSpeak:WriteApiKey"];

// Avsluta med ett tydligt meddelande om nyckeln saknas.
if (string.IsNullOrWhiteSpace (writeApiKey))
    {
    Console.WriteLine ("ThingSpeak API-nyckel saknas i User Secrets.");
    Console.WriteLine ("Tryck på valfri tangent för att avsluta.");
    Console.ReadKey ();
    return;
    }

Console.WriteLine ("ThingSpeak API-nyckel har lästs in.");

// Återanvänd samma HTTP-klient under hela körningen.
using var httpClient = new HttpClient
    {
    Timeout = TimeSpan.FromSeconds (10)
    };

// Koppla samman tjänsten för ThingSpeak med bilsimuleringen.
var thingSpeakService = new ThingSpeakService (httpClient, writeApiKey);
var simulator = new CarSimulationService (thingSpeakService);

await simulator.StartSimulation ();

// Håll konsolen öppen efter avslutad simulering.
Console.WriteLine ("Tryck på valfri tangent för att avsluta.");
Console.ReadKey ();