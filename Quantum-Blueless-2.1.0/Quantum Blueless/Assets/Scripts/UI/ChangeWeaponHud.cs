using Quantum;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This behavior handles the Change Weapon UI on mobile inputs
/// </summary>
public class ChangeWeaponHud : MonoBehaviour
{
  public Image weaponIconImage;

  unsafe void Update()
  {
    if (QuantumRunner.Default == null || QuantumRunner.Default.Game.Frames.Verified == null)
    {
      return;
    }
    var f = QuantumRunner.Default.Game.Frames.Verified;

    foreach (var (entity, player) in f.Unsafe.GetComponentBlockIterator<PlayerID>())
    {
      if (QuantumRunner.Default.Game.PlayerIsLocal(player->PlayerRef))
      {
        var wInventory = f.Get<WeaponInventory>(entity);
        var weapon = wInventory.Weapons[wInventory.CurrentWeaponIndex];
        var data = f.FindAsset<WeaponData>(weapon.WeaponData.Id);
        weaponIconImage.sprite = UnityDB.FindAsset<WeaponDataAsset>(data.Guid).uiIcon;
      }
    }
  }
}