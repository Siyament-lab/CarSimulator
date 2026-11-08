using System.Globalization;
using CarSimulator.Analysis.DTOs;
using CarSimulator.Analysis.Models;

namespace CarSimulator.Analysis.Services;

public class TelemetryAnalysisService
    {
    public TelemetryStatistics CalculateStatistics (
        List<ThingSpeakFeedDto> feeds )
        {
        var rpmValues = new List<double> ();
        var speedValues = new List<double> ();

        foreach (var feed in feeds)
            {
            // Tolka varvtalet och hoppa över saknade eller ogiltiga värden.
            if (TryParseValue (feed.Rpm, out var rpm) &&
                rpm >= 0 && rpm <= 6000)
                {
                rpmValues.Add (rpm);
                }

            // Tolka hastigheten och kontrollera simuleringens gränser.
            if (TryParseValue (feed.Speed, out var speed) &&
                speed >= 0 && speed <= 120)
                {
                speedValues.Add (speed);
                }
            }

        // Beräkna bara medelvärden när det finns giltiga värden.
        return new TelemetryStatistics
            {
            TotalReadings = feeds.Count,
            ValidRpmReadings = rpmValues.Count,
            ValidSpeedReadings = speedValues.Count,
            AverageRpm = rpmValues.Count > 0
                ? rpmValues.Average ()
                : null,
            AverageSpeed = speedValues.Count > 0
                ? speedValues.Average ()
                : null
            };
        }

    private bool TryParseValue ( string? text, out double value )
        {
        // ThingSpeak använder decimalpunkt oavsett datorns språk.
        // Uteslut även NaN och oändliga värden.
        return double.TryParse (
            text,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out value) && double.IsFinite (value);
        }
    }