using Photon.Deterministic;

namespace Quantum
{
  /// <summary>
  ///   Handles game timer
  /// </summary>
  public unsafe class GameControllerSystem : SystemMainThread, ISignalOnGameEnded
  {
    public override void OnInit(Frame f)
    {
      f.Global->GameController.GameTimer = FP._0;
      f.Global->GameController.State = GameState.Running;
    }

    public override void Update(Frame f)
    {
      var gameConfigData = f.FindAsset<GameControllerData>(f.RuntimeConfig.GameConfigData.Id);
      if (f.Global->GameController.GameTimer >= gameConfigData.GameDuration)
      {
        f.Signals.OnGameEnded(&f.Global->GameController);
        f.Events.OnGameEnded();
      }
      else
      {
        f.Global->GameController.GameTimer += f.DeltaTime;
      }
    }

    void ISignalOnGameEnded.OnGameEnded(Frame f, GameController* gameController)
    {
      f.Global->GameController.State = GameState.Ended;
      f.SystemDisable<GameControllerSystem>();
    }
  }
}