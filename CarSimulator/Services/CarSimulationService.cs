using carsimulator.Models;

namespace carsimulator.Services;

public class CarSimulationService
    {
    // Konstanter som styr simuleringens beteende.
    private const double MaxFuelConsumptionPerSecond = 0.005;
    private const int IdleRpm = 800;
    private const int MaxRpm = 6000;
    private const int MinSpeed = 0;
    private const int MaxSpeed = 120;
    private const int UpdateIntervalSeconds = 15;
    private const int TripDurationMinutes = 10;

    // Variabler som håller reda på bilens aktuella tillstånd.
    private double _currentSpeed = 0;
    private double _currentRpm = IdleRpm;
    private double _currentFuelLevel = 100.0;

    private readonly Random _random = new Random ();

    // Tjänsten som skickar bilens data till ThingSpeak.
    private readonly ThingSpeakService _thingSpeakService;

    public CarSimulationService ( ThingSpeakService thingSpeakService )
        {
        _thingSpeakService = thingSpeakService;
        }

    // Kör simuleringen under den angivna restiden eller tills användaren avbryter.
    public async Task StartSimulation (CancellationToken cancellationToken= default)
        {
        Console.WriteLine ("Startar bilsimulering...");
        var startTime = DateTime.UtcNow;

        while (!cancellationToken.IsCancellationRequested)
            {
            var elapsedTime = DateTime.UtcNow - startTime;

            if (elapsedTime.TotalMinutes >= TripDurationMinutes)
                {
                Console.WriteLine ("Simuleringen är klar. Restiden har uppnåtts.");
                break;
                }

            SimulateTrip ();

            // Använd vår separata modell i stället för en inre klass.
            var data = new CarTelemetry
                {
                Speed = Math.Round (_currentSpeed, 2),
                Rpm = Math.Round (_currentRpm, 2),
                FuelLevel = Math.Round (_currentFuelLevel, 2),
                EngineTemp = Math.Round (GenerateEngineTemperature (), 2),
                Timestamp = DateTimeOffset.UtcNow
                };

            Console.WriteLine (
                $"Tid: {elapsedTime.TotalSeconds:F0}s | " +
                $"Hastighet: {data.Speed:F0} km/h | " +
                $"RPM: {data.Rpm:F0} | " +
                $"Bränsle: {data.FuelLevel:F1}% | " +
                $"Motortemperatur: {data.EngineTemp:F1} °C");

            // Skicka aktuell avläsning innan vi väntar på nästa.
            await _thingSpeakService.SendTelemetryAsync (data);

            // Avbryt väntan när användaren trycker på Esc.
            try
                {
                await Task.Delay (
                    TimeSpan.FromSeconds (UpdateIntervalSeconds),
                    cancellationToken);
                }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                break;
                }
            }
        }

    // Simulerar bilens rörelse och förändringar i tillståndet.
    private void SimulateTrip ()
        {
        string tripPhase;

        if (_currentSpeed < 30)
            {
            tripPhase = "Accelerating";
            }
        else if (_currentSpeed < 90)
            {
            tripPhase = "Cruising";
            }
        else
            {
            tripPhase = "Decelerating";
            }

        // Anpassa hastighet och varvtal efter aktuell körfas.
        switch (tripPhase)
            {
            case "Accelerating":
                _currentSpeed += _random.Next (2, 5);
                _currentRpm = CalculateRpm (_currentSpeed);
                break;

            case "Cruising":
                _currentSpeed += _random.NextDouble () * 2 - 1;
                _currentRpm = CalculateRpm (_currentSpeed);
                break;

            case "Decelerating":
                _currentSpeed -= _random.Next (2, 5);
                _currentRpm = CalculateRpm (_currentSpeed);
                break;
            }

        // Begränsa hastigheten och säkerställ minst tomgångsvarvtal.
        _currentSpeed = Math.Max (MinSpeed, Math.Min (MaxSpeed, _currentSpeed));
        _currentRpm = Math.Max (IdleRpm, _currentRpm);

        // Beräkna bränsleförbrukningen enligt lärarens modell.
        double consumptionRate =
            (_currentSpeed / MaxSpeed) *
            (_currentRpm / MaxRpm) *
            MaxFuelConsumptionPerSecond;

        _currentFuelLevel -= consumptionRate * UpdateIntervalSeconds;
        _currentFuelLevel = Math.Max (0, _currentFuelLevel);
        }

    // Beräkna varvtalet utifrån hastigheten.
    private double CalculateRpm ( double speed )
        {
        if (speed < 1)
            {
            return IdleRpm;
            }

        return IdleRpm + (speed / MaxSpeed) * (MaxRpm - IdleRpm);
        }

    // Generera motortemperatur utifrån hastigheten.
    private double GenerateEngineTemperature ()
        {
        double baseTemp = 85;
        double speedFactor = _currentSpeed / MaxSpeed;
        double temp = baseTemp + (speedFactor * _random.Next (0, 15));

        return Math.Max (80, Math.Min (110, temp));
        }
    }