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

// Skapa en signal som kan stoppa simuleringen.
using var cancellation = new CancellationTokenSource ();

Console.WriteLine ("Tryck på Esc för att avbryta simuleringen.");

// Starta simuleringen och kontrollera tangentbordet medan den körs.
var simulationTask = simulator.StartSimulation (cancellation.Token);

while (!simulationTask.IsCompleted)
    {
    if (Console.KeyAvailable &&
        Console.ReadKey (intercept: true).Key == ConsoleKey.Escape)
        {
        Console.WriteLine ("Avbryter simuleringen...");
        cancellation.Cancel ();
        break;
        }

    // Kontrollera tangentbordet utan att belasta processorn i onödan.
    await Task.Delay (100);
    }

// Låt simuleringen avslutas innan programmet fortsätter.
await simulationTask;

// Håll konsolen öppen efter avslutad simulering.
Console.WriteLine ("Tryck på valfri tangent för att avsluta.");
Console.ReadKey ();