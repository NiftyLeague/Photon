using Photon.Deterministic;

namespace Quantum
{
  /// <summary>
  ///   Handles all skill entity interactions
  ///   Things this system handles:
  ///   - Skill Life Cycle
  ///   - Activation timers
  ///   - Skill projectile movement
  ///   - Skill collision checks
  /// </summary>
  public unsafe class SkillSystem : SystemMainThread
  {
    public override void Update(Frame f)
    {
      foreach (var (skill, skillFields) in f.Unsafe.GetComponentBlockIterator<SkillFields>())
      {
        if (skillFields->TimeToActivate <= FP._0)
        {
          DealAreaDamage(f, skill);
          f.Destroy(skill);
        }
        else
        {
          skillFields->TimeToActivate -= f.DeltaTime;
        }
      }
    }

    private static void DealAreaDamage(Frame f, EntityRef skill)
    {
      var skillTransform = f.Get<Transform2D>(skill);
      var skillData = f.FindAsset<SkillData>(f.Get<SkillFields>(skill).SkillData.Id);


      f.Events.OnSkillActivated(skillTransform.Position);

      var hits = f.Physics2D.OverlapShape(skillTransform, skillData.ShapeConfig.CreateShape(f));
      for (var i = 0; i < hits.Count; i++)
      {
        var entity = hits[i].Entity;

        if (entity == skill)
        {
          continue;
        }

        var skillFields = f.Get<SkillFields>(skill);
        var skillSourceEntity = skillFields.Source;

        // Only consider robots for damage
        if (entity.Equals(EntityRef.None) || f.Has<Status>(entity) == false)
        {
          continue;
        }

        // Only deal damages to robots not behind walls 
        var robotPosition = f.Get<Transform2D>(entity).Position;
        if (!LineOfSightHelper.HasLineOfSight(f, skillTransform.Position, robotPosition))
        {
          continue;
        }

        //Don't hit the caster robot!
        if (entity == skillSourceEntity)
        {
          continue;
        }
        f.Signals.OnRobotSkillHit(skill, entity);
        f.Events.OnSkillHitTarget(skillTransform.Position, skillFields.SkillData.Id.Value, entity);
      }
    }
  }
}