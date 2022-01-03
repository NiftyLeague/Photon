using Photon.Deterministic;

namespace Quantum {
  public static class CommandSetup {
    public static DeterministicCommand[] CreateCommands(RuntimeConfig gameConfig, SimulationConfig simulationConfig) {
      return new DeterministicCommand[] {
        // pre-defined core commands
        Core.DebugCommand.CreateCommand(),

        // user commands go here
      };
    }
  }
}
