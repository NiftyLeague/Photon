using Photon.Deterministic;

namespace Quantum
{
  /// <summary>
  ///   Manages health and status effects (such as invencibility and death)
  /// </summary>
  public unsafe class StatusSystem : SystemMainThread, ISignalOnRobotRespawn, ISignalOnRobotHit, ISignalOnRobotSkillHit
  {
    void ISignalOnRobotHit.OnRobotHit(Frame f, EntityRef bullet, EntityRef robot, FP damage)
    {
      var shooter = f.Get<BulletFields>(bullet).Source;
      TakeDamage(f, shooter, robot, damage);
    }


    public void OnRobotRespawn(Frame f, EntityRef robot)
    {
      var status = f.Unsafe.GetPointer<Status>(robot);
      var statusData = f.FindAsset<StatusData>(status->StatusData.Id);

      status->IsDead = false;
      status->CurrentHealth = statusData.MaxHealth;
      status->InvincibleTimer = statusData.InvincibleTime;
    }

    void ISignalOnRobotSkillHit.OnRobotSkillHit(Frame f, EntityRef skillRef, EntityRef robotRef)
    {
      var skillFields = f.Get<SkillFields>(skillRef);
      var skillData = f.FindAsset<SkillData>(skillFields.SkillData.Id);
      var caster = skillFields.Source;
      TakeDamage(f, caster, robotRef, skillData.Damage);
    }

    public override void Update(Frame f)
    {
      foreach (var (robot, status) in f.Unsafe.GetComponentBlockIterator<Status>())
      {
        var statusData = f.FindAsset<StatusData>(status->StatusData.Id);

        status->RegenTimer -= f.DeltaTime;
        if (status->RegenTimer < 0)
        {
          status->CurrentHealth += f.DeltaTime * statusData.RegenRate;
          status->CurrentHealth = FPMath.Clamp(status->CurrentHealth, status->CurrentHealth,
            statusData.MaxHealth);
        }

        if (status->InvincibleTimer > FP._0)
        {
          status->InvincibleTimer -= f.DeltaTime;
        }
      }
    }

    private static void TakeDamage(Frame f, EntityRef enemy, EntityRef robot, FP damage)
    {
      var robotStatus = f.Unsafe.GetPointer<Status>(robot);

      if (robotStatus->InvincibleTimer > FP._0 || damage < FP._1)
      {
        return;
      }

      var statusData = f.FindAsset<StatusData>(robotStatus->StatusData.Id);

      robotStatus->RegenTimer = statusData.TimeUntilRegen;
      robotStatus->CurrentHealth -= damage;
      f.Events.OnRobotTakeDamage(robot, damage, enemy);

      if (robotStatus->CurrentHealth <= 0)
      {
        KillRobot(f, enemy, robot, statusData.RespawnTime);
      }
    }

    private static void KillRobot(Frame f, EntityRef killer, EntityRef robot, FP respawnTime)
    {
      var robotStatus = f.Unsafe.GetPointer<Status>(robot);
      var characterController = f.Unsafe.GetPointer<CharacterController2D>(robot);
      var collider = f.Unsafe.GetPointer<PhysicsCollider2D>(robot);

      robotStatus->CurrentHealth = FP._0;
      robotStatus->IsDead = true;
      robotStatus->RespawnTimer = respawnTime;
      characterController->Velocity = FPVector2.Zero;
      collider->IsTrigger = true;

      f.Signals.OnRobotDeath(robot, killer);
      f.Events.OnRobotDeath(robot, killer);
    }
  }
}