  using Photon.Deterministic;

namespace Quantum
{
  /// <summary>
  ///   Handles input logic and creation of entities for the Skills
  /// </summary>
  public unsafe class SkillInventorySystem : SystemMainThread
  {
    public override void Update(Frame f)
    {
      var bulletFieldsFilter = f.Filter<PlayerID, Status, SkillInventory>();
      while (bulletFieldsFilter.NextUnsafe(out var robot, out var playerID, out var status, out var robotSkillInventory))
      {
        if (status->IsDead)
        {
          continue;
        }

        var input = f.GetPlayerInput(playerID->PlayerRef);

        if (robotSkillInventory->CastRateTimer <= FP._0)
        {
          if (input->CastSkill.WasPressed)
          {
            CastSkill(f, robot, input->AimDirection);
          }
        }
        else
        {
          robotSkillInventory->CastRateTimer -= f.DeltaTime;
        }
      }
    }

    /// <summary>
    /// Creates a new Skill Entity in the world and setup it's values
    /// </summary>
    /// <param name="f"></param>
    /// <param name="robot"></param>
    /// <param name="angle"></param>
    private void CastSkill(Frame f, EntityRef robot, FP angle)
    {
      var robotPosition = f.Get<Transform2D>(robot).Position;

      var skillPrototype = f.FindAsset<EntityPrototype>(new AssetGuid(4473321660304365115));
      var skill = f.Create(skillPrototype);

      var skillInventory = f.Unsafe.GetPointer<SkillInventory>(robot);
      var skillInventoryData = f.FindAsset<SkillInventoryData>(skillInventory->SkillInventoryData.Id);
      var skillData = f.FindAsset<SkillData>(skillInventoryData.SkillData.Id);

      var skillFields = f.Unsafe.GetPointer<SkillFields>(skill);

      skillFields->SkillData = skillData;
      skillFields->Source = robot;
      skillFields->TimeToActivate = skillData.ActivationDelay;

      var skillTransform = f.Unsafe.GetPointer<Transform2D>(skill);
      skillTransform->Position = robotPosition;

      var skillPhysics = f.Unsafe.GetPointer<PhysicsBody2D>(skill);

      skillPhysics->Velocity = FPVector2.Rotate(FPVector2.Right, angle) * skillInventoryData.CastForce;
      skillInventory->CastRateTimer = skillInventoryData.CastRate;

      f.Events.OnSkillCasted(skill);
    }
  }
}