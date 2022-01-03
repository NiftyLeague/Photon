using Photon.Deterministic;

namespace Quantum
{
  /// <summary>
  ///   Handles all bullet entity interactions
  ///   Things this system handles:
  ///   - Bullet Life Cycle
  ///   - Bullet Movement
  ///   - Bullet Collision (via Raycast)
  /// </summary>
  public unsafe class BulletSystem : SystemMainThread, ISignalOnGameEnded
  {
    void ISignalOnGameEnded.OnGameEnded(Frame f, GameController* gameController)
    {
      f.SystemDisable<BulletSystem>();
    }
    public override void Update(Frame f)
    {
      var bulletFieldsFilter = f.Filter<Transform2D, BulletFields>();
      while (bulletFieldsFilter.NextUnsafe(out var bullet, out var bulletTransform, out var bulletFields))
      {
        if (RaycastCollision(f, bullet))
        {
          continue;
        }

        bulletTransform->Position += bulletFields->Direction * f.DeltaTime;
        bulletFields->Time += f.DeltaTime;

        var sourcePosition = f.Get<Transform2D>(bulletFields->Source).Position;
        var bulletData = f.FindAsset<BulletData>(bulletFields->BulletData.Id);

        var bulletIsTooFar = FPVector2.Distance(bulletTransform->Position, sourcePosition) > bulletData.Range;
        var bulletIsOld = bulletData.Duration > FP._0 && bulletFields->Time >= bulletData.Duration;

        if (bulletIsTooFar || bulletIsOld)
        {
          // Applies polymorphic behavior on the bullet action
          bulletData.BulletAction(f, bullet, EntityRef.None);
        }
      }
    }

    private bool RaycastCollision(Frame f, EntityRef bullet)
    {
      var bulletFields = f.Get<BulletFields>(bullet);

      if (bulletFields.Direction.Magnitude <= 0)
      {
        return false;
      }

      var bulletTransform = f.Get<Transform2D>(bullet);
      var futurePosition = bulletTransform.Position + bulletFields.Direction * f.DeltaTime;
      var data = f.FindAsset<BulletData>(bulletFields.BulletData.Id);

      if (FPVector2.DistanceSquared(bulletTransform.Position, futurePosition) <= FP._0_01)
      {
        return false;
      }

      //using (var hits = f.Scene.Linecastc)
      var hits = f.Physics2D.LinecastAll(bulletTransform.Position, futurePosition);
      for (var i = 0; i < hits.Count; i++)
      {
        var entity = hits[i].Entity;

        if (entity != EntityRef.None && f.Has<Status>(entity) && entity != bulletFields.Source)
        {
          if (f.Get<Status>(entity).IsDead) {
            continue;
          }
          bulletTransform.Position = hits[i].Point;
          // Applies polymorphic behavior on the bullet action
          data.BulletAction(f, bullet, entity);
          f.Set(bullet, bulletTransform);
          return true;
        }

        if (entity == EntityRef.None && !data.bounce)
        {
          bulletTransform.Position = hits[i].Point;
          // Applies polymorphic behavior on the bullet action
          data.BulletAction(f, bullet, EntityRef.None);
          f.Set(bullet, bulletTransform);
          return true;
        }
      }

      return false;
    }
  }
}