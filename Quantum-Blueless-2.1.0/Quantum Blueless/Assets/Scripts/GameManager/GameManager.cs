using Quantum;

public class GameManager : QuantumCallbacks
{
  public QuantumGame Game => QuantumRunner.Default.Game;

  private EntityView _localView;
  public EntityView LocalView { get { return _localView; } }

  private EntityViewsManager _viewsManager;
  public EntityViewsManager ViewsManager { get { return _viewsManager; } }

  protected override void OnEnable()
  {
    base.OnEnable();

    _viewsManager = FindObjectOfType<EntityViewsManager>();
    _viewsManager.Init(this);
  }

  public void OnEntityViewCreated(EntityView entityView)
  {
    QuantumGame game = QuantumRunner.Default.Game;
    Frame frame = game.Frames.Predicted;

    if (frame.Has<PlayerID>(entityView.EntityRef) == false)
    {
      return;
    }

    if (entityView is GameEntityView gameEntityView)
    {
      gameEntityView.Init(this);
    }


    PlayerID playerLink = frame.Get<PlayerID>(entityView.EntityRef);
    if (game.PlayerIsLocal(playerLink.PlayerRef) == true)
    {
      _localView = entityView;
    }
  }
}
