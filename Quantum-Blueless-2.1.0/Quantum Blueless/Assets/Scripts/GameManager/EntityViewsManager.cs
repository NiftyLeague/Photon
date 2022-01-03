using Quantum;
using UnityEngine;

public class EntityViewsManager : EntityViewUpdater
{
  private GameManager _gameManager;

  public void Init(GameManager gameManager)
  {
    _gameManager = gameManager;
  }

  protected override void OnEntityViewInstantiated(QuantumGame game, Frame f, EntityView instance, EntityRef handle)
  {
    base.OnEntityViewInstantiated(game, f, instance, handle);

    if (instance is IEntityView entityViewInterface)
    {
      entityViewInterface.Init(_gameManager);
    }

    _gameManager.OnEntityViewCreated(instance);
  }
}
