using Photon.Deterministic;

namespace Quantum
{
  /// <summary>
  /// Handles changing weapon
  /// </summary>
  public unsafe class WeaponInventorySystem : SystemMainThread, ISignalOnGameEnded
  {
    public override void Update(Frame f)
    {
      var bulletFieldsFilter = f.Filter<PlayerID, Status>();
      while (bulletFieldsFilter.NextUnsafe(out var robot, out var playerID, out var status))
      {
        if (status->IsDead)
        {
          continue;
        }

        Input* input = f.GetPlayerInput(playerID->PlayerRef);
        if (input->ChangeWeapon.WasPressed)
        {
          ChangeWeapon(f, robot);
        }
      }
    }

    private void ChangeWeapon(Frame f, EntityRef robot)
    {
      var weaponInventory = f.Unsafe.GetPointer<WeaponInventory>(robot);
      weaponInventory->CurrentWeaponIndex = (weaponInventory->CurrentWeaponIndex + 1) % weaponInventory->Weapons.Length;
      Weapon* currentWeapon = weaponInventory->Weapons.GetPointer(weaponInventory->CurrentWeaponIndex);
      currentWeapon->chargeTime = FP._0;

      f.Events.OnRobotChangeWeapon(robot);
    }

    void ISignalOnGameEnded.OnGameEnded(Frame f, GameController* gameController)
    {
      f.SystemDisable<WeaponInventorySystem>();
    }
  }
}