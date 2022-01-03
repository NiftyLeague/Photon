using Photon.Deterministic;

namespace Quantum
{
  public static unsafe class LineOfSightHelper
  {
    // Returns true if there's no static collider between source and target
    public static bool HasLineOfSight(Frame f, FPVector2 source, FPVector2 target)
    {
      var hits = f.Physics2D.LinecastAll(source, target);
      for (var j = 0; j < hits.Count; j++)
      {
        var e = hits[j].Entity;
        if (e == EntityRef.None) {
          return false;
        }
      }
      return true;
    }
  }
}