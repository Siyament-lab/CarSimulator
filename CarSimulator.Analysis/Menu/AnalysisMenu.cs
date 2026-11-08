using CarSimulator.Analysis.DTOs;
using CarSimulator.Analysis.Services;

namespace CarSimulator.Analysis.Menu;

public class AnalysisMenu
    {
    private readonly ThingSpeakService _thingSpeakService;
    private readonly TelemetryAnalysisService _analysisService;

    public AnalysisMenu (
        ThingSpeakService thingSpeakService,
        TelemetryAnalysisService analysisService )
        {
        _thingSpeakService = thingSpeakService;
        _analysisService = analysisService;
        }

    public async Task ShowAsync ()
        {
        while (true)
            {
            Console.WriteLine ();
            Console.WriteLine ("=== Analys av bilens telemetri ===");
            Console.WriteLine ("1. Senaste 24 timmarna");
            Console.WriteLine ("2. Senaste 100 avläsningarna");
            Console.WriteLine ("0. Avsluta");
            Console.Write ("Välj ett alternativ: ");

            var choice = Console.ReadLine ()?.Trim ();

            // Avsluta även om konsolens inmatning stängs.
            if (choice is null || choice == "0")
                {
                return;
                }

            List<ThingSpeakFeedDto>? feeds;

            // Hämta data utifrån användarens val.
            switch (choice)
                {
                case "1":
                    Console.WriteLine ("Hämtar de senaste 24 timmarna...");
                    feeds = await _thingSpeakService.GetLast24HoursAsync ();
                    break;

                case "2":
                    Console.WriteLine ("Hämtar de senaste 100 avläsningarna...");
                    feeds = await _thingSpeakService.GetLast100Async ();
                    break;

                default:
                    Console.WriteLine ("Ogiltigt val. Ange 1, 2 eller 0.");
                    continue;
                }

            // Tjänsten har redan skrivit ut felet om hämtningen misslyckats.
            if (feeds is null)
                {
                continue;
                }

            if (feeds.Count == 0)
                {
                Console.WriteLine ("Inga avläsningar hittades för ditt val.");
                continue;
                }

            ShowStatistics (feeds);
            }
        }

    private void ShowStatistics ( List<ThingSpeakFeedDto> feeds )
        {
        // Beräkna och presentera statistik för vald period.
        var statistics = _analysisService.CalculateStatistics (feeds);

        Console.WriteLine ();
        Console.WriteLine ($"Antal avläsningar: {statistics.TotalReadings}");

        Console.WriteLine (statistics.AverageRpm.HasValue
            ? $"Genomsnittligt varvtal: {statistics.AverageRpm.Value:F1} RPM"
            : "Genomsnittligt varvtal: saknar giltiga värden.");

        Console.WriteLine (statistics.AverageSpeed.HasValue
            ? $"Genomsnittlig hastighet: {statistics.AverageSpeed.Value:F1} km/h"
            : "Genomsnittlig hastighet: saknar giltiga värden.");

        Console.WriteLine ($"Giltiga RPM-värden: {statistics.ValidRpmReadings}");
        Console.WriteLine ($"Giltiga hastighetsvärden: {statistics.ValidSpeedReadings}");
        }
    }