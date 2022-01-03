using Photon.Deterministic;
namespace Quantum
{
  public static unsafe class RespawnHelper
  {
    public static void RespawnRobot(Frame f, EntityRef robot)
    {
      var position = FPVector2.One * 4;

      var spawnCount = f.ComponentCount<SpawnIdentifier>();
      if (spawnCount != 0)
      {
        var index = f.RNG->Next(0, spawnCount);
        var count = 0;
        foreach (var (spawn, spawnIdentifier) in f.Unsafe.GetComponentBlockIterator<SpawnIdentifier>())
        {
          if (count == index)
          {
            var spawnTransform = f.Get<Transform2D>(spawn);
            position = spawnTransform.Position;
            break;
          }
          count++;
        }
      }

      var robotTransform = f.Unsafe.GetPointer<Transform2D>(robot);
      var collider = f.Unsafe.GetPointer<PhysicsCollider2D>(robot);

      robotTransform->Position = position;
      collider->IsTrigger = false;

      f.Signals.OnRobotRespawn(robot);
      f.Events.OnRobotRespawn(robot);
    }
  }
}