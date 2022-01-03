using System;
using Photon.Deterministic;

namespace Quantum
{
  public unsafe class DisconnectSystem : SystemMainThread
  {
    public override void Update(Frame f)
    {
      f.Global->DisconnectTime += f.DeltaTime;

      var robotsFilter = f.Filter<PlayerID, Status>();
      while (robotsFilter.NextUnsafe(out var robot, out var playerID, out var robotStatus))
      {
        var flags = f.GetPlayerInputFlags(playerID->PlayerRef);

        if ((flags & DeterministicInputFlags.PlayerNotPresent) == DeterministicInputFlags.PlayerNotPresent)
        {
          robotStatus->DisconnectedTicks++;
        }
        else
        {
          robotStatus->DisconnectedTicks = 0;
        }

        if (robotStatus->DisconnectedTicks >= 15)
        {
          f.Destroy(robot);
        }
      }
    }
  }
}
