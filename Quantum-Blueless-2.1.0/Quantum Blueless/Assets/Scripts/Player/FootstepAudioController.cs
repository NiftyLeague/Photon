using UnityEngine;
using Quantum;

public unsafe class FootstepAudioController : QuantumCallbacks
{
	public PlayerAudioController audioController;
	public float stepsDelay;
	public float velocityThreshold = 0.5f;
	
	private float timer;
	public GameManager gameManager;
	public EntityView entityView;


	protected override void OnEnable()
	{
		base.OnEnable();
		gameManager = FindObjectOfType<GameManager>();
		entityView = GetComponentInParent<EntityView>();
	}

	public override void OnUpdateView(QuantumGame game)
	{
		var f = gameManager.Game.Frames.Verified;
		var kcc = f.Get<CharacterController2D>(entityView.EntityRef);
		if (kcc.Grounded && Mathf.Abs(kcc.Velocity.X.AsFloat) > velocityThreshold)
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
				PlayFootstep();
		}
	}

	public void Update()
	{
  }

	public void PlayFootstep()
	{
		timer = stepsDelay;
		audioController.OnFootStep();
	}
}