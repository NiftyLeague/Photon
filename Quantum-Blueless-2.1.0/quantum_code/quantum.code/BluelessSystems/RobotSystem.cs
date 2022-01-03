using Photon.Deterministic;
using Quantum.Task;

namespace Quantum
{
  /// <summary>
  ///   Handles creation of robots
  /// </summary>
  public unsafe class RobotSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
  {
    void ISignalOnPlayerDataSet.OnPlayerDataSet(Frame f, PlayerRef player)
    {
      var data = f.GetPlayerData(player);
      var prototypeAsset = f.FindAsset<EntityPrototype>(data.PrototypeRef.Id.Value);
      var robot = f.Create(prototypeAsset);

      var playerID = f.Unsafe.GetPointer<PlayerID>(robot);
      playerID->PlayerRef = player;

      RespawnHelper.RespawnRobot(f, robot);

      f.Events.OnRobotCreated(robot);
      f.Signals.OnRobotRespawn(robot);
    }
  }
}