using Photon.Deterministic;

namespace Quantum
{
  /// <summary>
  ///   Handles player scores (through signals)
  /// </summary>
  public unsafe class ScoreSystem : SystemSignalsOnly, ISignalOnRobotDeath
  {
    void ISignalOnRobotDeath.OnRobotDeath(Frame f, EntityRef deadRef, EntityRef killerRef)
    {
      var killerScore = f.Unsafe.GetPointer<Score>(killerRef);
      var deadScore = f.Unsafe.GetPointer<Score>(deadRef);

      if (killerRef != deadRef)
      {
        killerScore->Kills += 1;
      }
      deadScore->Deaths += 1;
    }
  }
}