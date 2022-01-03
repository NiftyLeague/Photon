using Quantum;
using UnityEngine;

public class GameEntityView : EntityView, IEntityView
{
  private GameManager gameManager;

  public bool IsLocal => EntityRef == gameManager.LocalView.EntityRef;

  public void Init(GameManager gameManager)
  {
    gameManager = gameManager;
  }
}
