
namespace Quantum
{
  /// <summary>
  /// Normal bullet behavior
  /// 
  /// Deals damage on a robot.
  /// </summary>
  [System.Serializable]
  public partial class CommonBulletData : BulletData
  {
    public override unsafe void BulletAction(Frame f, EntityRef bullet, EntityRef targetRobot)
    {
      if (targetRobot != EntityRef.None)
      {
        f.Signals.OnRobotHit(bullet, targetRobot, Damage);
      }

      var fields = f.Get<BulletFields>(bullet);
      var position = f.Get<Transform2D>(bullet).Position;
      f.Events.OnBulletDestroyed(bullet.GetHashCode(), fields.Source, position, fields.Direction, fields.BulletData.Id.Value);
      f.Destroy(bullet);
    }
  }
}