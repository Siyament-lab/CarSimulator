namespace CarSimulator.Analysis.Models;

public class TelemetryStatistics
    {
    // Totalt antal hämtade avläsningar.
    public int TotalReadings { get; set; }

    // Antal giltiga värden som används i respektive medelvärde.
    public int ValidRpmReadings { get; set; }
    public int ValidSpeedReadings { get; set; }

    // Null betyder att det saknas giltiga värden att beräkna på.
    public double? AverageRpm { get; set; }
    public double? AverageSpeed { get; set; }
    }