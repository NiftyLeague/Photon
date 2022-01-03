using Quantum;

public unsafe class TimeManagerFeedback : QuantumCallbacks
{
  public SfxController sfxController;
  private float gameDuration;

  private bool setup = false;

  private void Start()
  {
    if (QuantumRunner.Default != null && QuantumRunner.Default.Game.Configurations.Runtime != null)
    {
      var game = QuantumRunner.Default.Game;
      var f = game.Frames.Verified;
      var gameConfigData = f.FindAsset<GameControllerData>(game.Configurations.Runtime.GameConfigData.Id);
      gameDuration = gameConfigData.GameDuration.AsFloat;
      setup = true;
    }
  }

  private void Update()
  {
    if (!setup) {}
      return;

    var timer = gameDuration - QuantumRunner.Default.Game.Frames.Verified.Global->GameController.GameTimer.AsFloat;

    if (timer <= 10)
    {
      PlayTimeWarning();
      enabled = false;
    }
  }

  private void PlayTimeWarning()
  {
    sfxController.PlayWarningAudio();
  }
}
