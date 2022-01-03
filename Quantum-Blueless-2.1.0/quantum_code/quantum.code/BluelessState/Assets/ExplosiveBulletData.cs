using System;
using Photon.Deterministic;

namespace Quantum
{
  /// <summary>
  ///   Explosive bullet behavior
  ///   Explodes on impact, deals damage to all robots in the ExposionRadius.
  /// </summary>
  [Serializable]
  public partial class ExplosiveBulletData : BulletData
  {
    public Shape2DConfig ExplosionShape;
    
    public override unsafe void BulletAction(Frame frame, EntityRef bullet, EntityRef targetRobot)
    {
      Explode(frame, bullet, targetRobot);

      if (targetRobot != EntityRef.None)
      {
        frame.Signals.OnRobotHit(bullet, targetRobot, Damage);
      }

      var bulletFields = frame.Get<BulletFields>(bullet);
      var bulletPosition = frame.Get<Transform2D>(bullet).Position;

      frame.Events.OnBulletDestroyed(bullet.GetHashCode(), bulletFields.Source, bulletPosition, bulletFields.Direction, Guid.Value);
      frame.Destroy(bullet);
    }

    private unsafe void Explode(Frame f, EntityRef bullet, EntityRef robot)
    {
      var bulletTransform = f.Get<Transform2D>(bullet);

      var hits = f.Physics2D.OverlapShape(bulletTransform, ExplosionShape.CreateShape(f));
      for (var i = 0; i < hits.Count; i++)
      {
        var entity = hits[i].Entity;

        // Only consider robots for damage
        if (entity.Equals(EntityRef.None) || f.Has<Status>(entity) == false)
        {
          continue;
        }

        // Only deal damages to robots not behind walls 
        var currentBotTransform = f.Get<Transform2D>(entity);
        if (!LineOfSightHelper.HasLineOfSight(f, bulletTransform.Position, currentBotTransform.Position))
        {
          continue;
        }

        // Don't hit the target robot, we deal his damage in another place so it doesnt suffer falloff
        if (entity.Equals(robot))
        {
          continue;
        }

        var distance = FPVector2.Distance(bulletTransform.Position, currentBotTransform.Position);
        var damagePercentage = 1 - distance / (ExplosionShape.CircleRadius + ShapeConfig.CircleRadius);
        damagePercentage = FPMath.Clamp01(damagePercentage);
        var damage = Damage * damagePercentage;

        f.Signals.OnRobotHit(bullet, entity, damage);
      }
    }
  }
}