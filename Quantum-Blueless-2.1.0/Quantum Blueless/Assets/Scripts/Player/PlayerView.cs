using Quantum;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Handles player model rotation to face the right way
/// </summary>
public sealed unsafe class PlayerView : QuantumCallbacks
{
	public static System.Action<PlayerView> onLocalPlayerInstantiated;
	private static Dictionary<EntityRef, PlayerView> playerViewDictionary = new Dictionary<EntityRef, PlayerView>();

	public Transform body;
	public Vector3 rightRotation;
	public Vector3 leftRotation;
	public int lookDirection;

	private EntityView entityView;
	private GameManager gameManager;

	protected override void OnEnable()
	{
		base.OnEnable();
		gameManager = FindObjectOfType<GameManager>();
		entityView = GetComponentInParent<EntityView>();
	}

	private void Start()
	{
		var f = gameManager.Game.Frames.Verified;
		var playerID = f.Get<PlayerID>(entityView.EntityRef);
		playerViewDictionary[entityView.EntityRef] = this;
		if (gameManager.Game.PlayerIsLocal(playerID.PlayerRef))
		{
			onLocalPlayerInstantiated?.Invoke(this);
		}
	}

	public override void OnUpdateView(QuantumGame game)
	{
		var f = gameManager.Game.Frames.Verified;
		var robotMovement = f.Get<Movement>(entityView.EntityRef);
		if (robotMovement.IsFacingRight)
		{
			body.localRotation = Quaternion.Euler(rightRotation);
			lookDirection = 1;
		}
		else
		{
			body.localRotation = Quaternion.Euler(leftRotation);
			lookDirection = -1;
		}
	}
	public static PlayerView GetPlayerView(EntityRef entityRef)
	{
		if (playerViewDictionary.ContainsKey(entityRef))
		{
			return playerViewDictionary[entityRef];
		}
		return null;
	}
}