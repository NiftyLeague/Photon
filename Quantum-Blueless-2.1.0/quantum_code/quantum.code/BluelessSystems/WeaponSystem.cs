using Photon.Deterministic;

namespace Quantum
{
  /// <summary>
  /// Handles all things weapon related
  ///   Things this system handles:
  ///   - Weapon ammo recharge 
  ///   - Firing bullets
  /// </summary>
  public unsafe class WeaponSystem : SystemMainThread, ISignalOnRobotRespawn, ISignalOnGameEnded
  {
    void ISignalOnGameEnded.OnGameEnded(Frame f, GameController* gameController)
    {
      f.SystemDisable<WeaponSystem>();
    }

    void ISignalOnRobotRespawn.OnRobotRespawn(Frame f, EntityRef robot)
    {
      var weaponInventory = f.Get<WeaponInventory>(robot);

      for (var i = 0; i < weaponInventory.Weapons.Length; i++)
      {
        var weapon = weaponInventory.Weapons[i];
        var weaponData = f.FindAsset<WeaponData>(weapon.WeaponData.Id);

        weapon.IsRecharging = false;
        weapon.currentAmmo = weaponData.MaxAmmo;
        weapon.FireRateTimer = FP._0;
        weapon.DelayToStartRechargeTimer = FP._0;
        weapon.RechargeRate = FP._0;
      }

      f.Set(robot, weaponInventory);
    }

    public override void Update(Frame f)
    {
      var robotsFilter = f.Filter<PlayerID, Transform2D, Status, WeaponInventory>();


      while (robotsFilter.NextUnsafe(out var robot, out var playerID, out var transform2D, out var status, out var weaponInventory))
      {
        if (status->IsDead)
        {
          continue;
        }
        Weapon* currentWeapon = weaponInventory->Weapons.GetPointer(weaponInventory->CurrentWeaponIndex);

        currentWeapon->FireRateTimer -= f.DeltaTime;
        currentWeapon->DelayToStartRechargeTimer -= f.DeltaTime;
        currentWeapon->RechargeRate -= f.DeltaTime;

        var weaponData = f.FindAsset<WeaponData>(currentWeapon->WeaponData.Id);


        if (currentWeapon->DelayToStartRechargeTimer < 0 && currentWeapon->RechargeRate <= 0 &&
            currentWeapon->currentAmmo < weaponData.MaxAmmo)
        {
          IncreaseAmmo(f, currentWeapon, weaponData);
        }

        if (currentWeapon->FireRateTimer <= FP._0 && !currentWeapon->IsRecharging && currentWeapon->currentAmmo > 0)
        {
          var i = f.GetPlayerInput(playerID->PlayerRef);
          var isChargeWeapon = weaponData.IsChargeWeapon;

          if (isChargeWeapon && i->Fire.IsDown)
          {
            currentWeapon->chargeTime = FPMath.Min(
              currentWeapon->chargeTime + f.DeltaTime,
              weaponData.chargeDuration
            );
          }

          if (!isChargeWeapon && i->Fire.IsDown || isChargeWeapon && i->Fire.WasReleased)
          {
            SpawnBullet(f, robot, currentWeapon, i->AimDirection);
            currentWeapon->FireRateTimer = FP._1 / weaponData.FireRate;
            currentWeapon->chargeTime = FP._0;
          }
        }
      }
    }

    private static void IncreaseAmmo(Frame f, Weapon* weapon, WeaponData data)
    {
      weapon->RechargeRate = data.RechargeTimer / (FP)data.MaxAmmo;
      weapon->currentAmmo++;

      if (weapon->currentAmmo == data.MaxAmmo)
      {
        weapon->IsRecharging = false;
      }
    }

    private static void SpawnBullet(Frame f, EntityRef robot, Weapon* weapon, FP angle)
    {
      weapon->currentAmmo -= 1;

      if (weapon->currentAmmo == 0)
      {
        weapon->IsRecharging = true;
        weapon->DelayToStartRechargeTimer = -1;
      }

      var weaponData = f.FindAsset<WeaponData>(weapon->WeaponData.Id);


      weapon->DelayToStartRechargeTimer = weaponData.TimeToRecharge;

      f.Events.OnWeaponShoot(robot);

      BulletData bulletData = f.FindAsset<BulletData>(weaponData.BulletData.Id);
      
      var prototypeAsset = f.FindAsset<EntityPrototype>(new AssetGuid(bulletData.BulletPrototype.Id.Value));
      var bullet = f.Create(prototypeAsset);

      var bulletFields = f.Unsafe.GetPointer<BulletFields>(bullet);
      var bulletTransform = f.Unsafe.GetPointer<Transform2D>(bullet);

      bulletFields->BulletData = bulletData;

      var robotMovement = f.Unsafe.GetPointer<Movement>(robot);
      var robotTransform = f.Unsafe.GetPointer<Transform2D>(robot);

      var fireSpotWorldOffset = WeaponHelper.GetFireSpotWorldOffset(
        f.FindAsset<WeaponData>(weapon->WeaponData.Id),
        angle,
        robotMovement->IsFacingRight
      );

      bulletTransform->Position = robotTransform->Position + fireSpotWorldOffset;

      bulletFields->Direction = FPVector2.Rotate(FPVector2.Right, angle) * weaponData.ShootForce;
      bulletFields->Source = robot;
      bulletFields->Time = FP._0;
    }
  }
}