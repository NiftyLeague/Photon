using Quantum;
using TMPro;
using UnityEngine;

public class GameTimerHud : MonoBehaviour
{
  public TMP_Text timerText;

  void Start()
  {
    timerText.text = "";
  }

  void Update()
  {
    if (QuantumRunner.Default == null || QuantumRunner.Default.Game.Frames.Verified == null)
    {
      return;
    }

    UpdateTimer();
  }

  private unsafe void UpdateTimer()
  {
    var f = QuantumRunner.Default.Game.Frames.Verified;
    var gameConfigData = f.FindAsset<GameControllerData>(QuantumRunner.Default.Game.Configurations.Runtime.GameConfigData.Id);

    var gameController = f.Global->GameController;
    var gameTime = gameController.GameTimer.AsInt;
    var gameDuration = gameConfigData.GameDuration.AsInt;

    if (gameController.State == GameState.Ended)
    {
      timerText.text = "GAME OVER";
    }
    else
    {
      var timeLeft = gameDuration - gameTime;
      timerText.text = string.Format("{0:00}:{1:00}", timeLeft / 60, timeLeft % 60);
    }
  }
}
