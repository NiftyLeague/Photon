using Photon.Deterministic;
using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This behavior handles the Skill UI, showing cooldowns and etc
/// </summary>
public class SkillHud : MonoBehaviour
{
  public GameObject cooldownObject;
  public Image cooldownFill;
  public TMP_Text cooldownText;

  unsafe void Update()
  {
    if (QuantumRunner.Default == null || QuantumRunner.Default.Game.Frames.Verified == null)
    {
      return;
    }

    var f = QuantumRunner.Default.Game.Frames.Verified;
    foreach (var (robot, player) in f.Unsafe.GetComponentBlockIterator<PlayerID>())
    {
      if (QuantumRunner.Default.Game.PlayerIsLocal(player->PlayerRef))
      {
        var skillInventory = f.Get<SkillInventory>(robot);
        var data = f.FindAsset<SkillInventoryData>(skillInventory.SkillInventoryData.Id);
        var cooldownLeft = skillInventory.CastRateTimer;
        var cooldownMax = data.CastRate;

        cooldownObject.SetActive(cooldownLeft > FP._0);
        cooldownFill.fillAmount = (cooldownLeft / cooldownMax).AsFloat;
        cooldownText.text = Mathf.CeilToInt(cooldownLeft.AsFloat).ToString("0");
      }
    }
  }
}
