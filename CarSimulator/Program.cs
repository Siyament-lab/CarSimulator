using carsimulator.Services;

// Skapa och starta bilsimuleringen.
var simulator = new CarSimulationService ();
await simulator.StartSimulation ();

// Håll konsolen öppen efter avslutad simulering.
Console.WriteLine ("Tryck på valfri tangent för att avsluta.");
Console.ReadKey ();