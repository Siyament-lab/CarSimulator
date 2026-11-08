using carsimulator.Models;

namespace carsimulator.Interfaces;

public interface ICarSimulationService
    {
    // Uppdaterar bilens tillstånd och returnerar en ny telemetriavläsning.
    CarTelemetry GenerateTelemetry ();
    }