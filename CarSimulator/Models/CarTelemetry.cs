namespace carsimulator.Models;

public class CarTelemetry
    {
    // Motorns varvtal i varv per minut.
    public double Rpm { get; set; }

    // Bilens hastighet i km/h.
    public double Speed { get; set; }

    // Bränslenivå i procent.
    public double FuelLevel { get; set; }

    // Motortemperatur i grader Celsius.
    public double EngineTemp { get; set; }

    // Tidpunkten för avläsningen, lagrad i UTC.
    public DateTimeOffset Timestamp { get; set; }
    }