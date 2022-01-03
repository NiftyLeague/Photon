using Photon.Deterministic;

namespace Quantum
{
  /// <summary>
  ///   Handles respawn logic (Timer and actually respawning)
  /// </summary>
  public unsafe class RespawnSystem : SystemMainThread
  {
    public override void Update(Frame f)
    {
      foreach (var (robot, robotStatus) in f.Unsafe.GetComponentBlockIterator<Status>())
      {
        if (robotStatus->IsDead)
        {
          robotStatus->RespawnTimer -= f.DeltaTime;
          if (robotStatus->RespawnTimer <= FP._0)
          {
            RespawnHelper.RespawnRobot(f, robot);
          }
        }
      }
    }
  }
}