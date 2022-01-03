using System.Collections.Generic;
using Quantum;
using UnityEngine;
using AudioConfiguration = SfxController.AudioConfiguration;

/// <summary>
/// This Behavior handles events related to player actions that require audio feedback, such as Jump, Double Jumping, Death and Respawn
/// 
/// Uses the default Unity Audio API for simplicity
/// </summary>
public unsafe class PlayerAudioController : QuantumCallbacks
{
	[Header("References")]
	public AudioListener audioListener;

	public AudioSource audioSourcePrefab;

	[Header("Configurations")]
	public int maxAudioSources = 8;
	public Transform audioSourceParent;

	[Header("Audio Refs")]
	public AudioConfiguration jumpAudioClip;
	public AudioConfiguration doubleJumpAudioClip;
	public AudioConfiguration landingAudioClip;
	public AudioConfiguration deathClip;
	public AudioConfiguration respawnClip;
	public AudioConfiguration footstepsClip;

	private readonly Stack<AudioSource> freeAudioSources = new Stack<AudioSource>();
	private List<AudioSource> audioSourcesInUse = new List<AudioSource>();

	private GameManager gameManager;
	private EntityView entityView;

	protected override void OnEnable()
	{
		base.OnEnable();
		gameManager = FindObjectOfType<GameManager>();
		entityView = GetComponentInParent<EntityView>();
	}

	void Start()
	{

		for (int i = 0; i < maxAudioSources; i++)
		{
			var audioSource = Instantiate(audioSourcePrefab, audioSourceParent);
			audioSource.transform.localPosition = Vector3.zero;

			freeAudioSources.Push(audioSource);
		}

		// Register for relevant events		
		QuantumEvent.Subscribe<EventOnRobotJump>(this, OnJump);
		QuantumEvent.Subscribe<EventOnRobotDoubleJump>(this, OnDoubleJump);
		QuantumEvent.Subscribe<EventOnRobotGrounded>(this, OnLanding);
		QuantumEvent.Subscribe<EventOnRobotDeath>(this, OnDeath);
		QuantumEvent.Subscribe<EventOnRobotRespawn>(this, OnRespawn);
		CheckLocalAudioListener();
	}
	private void OnDestroy()
	{
		QuantumEvent.UnsubscribeListener(this);
	}

	private void CheckLocalAudioListener()
	{
		var f = gameManager.Game.Frames.Verified;
		if(f.Exists(entityView.EntityRef) == false) { return; }

		var player = f.Get<PlayerID>(entityView.EntityRef);
		if (gameManager.Game.PlayerIsLocal(player.PlayerRef))
		{
			audioListener.enabled = true;
		}
	}

	void Update()
	{
		for (var i = audioSourcesInUse.Count - 1; i >= 0; i--)
		{
			var source = audioSourcesInUse[i];
			if (!source.isPlaying)
			{
				freeAudioSources.Push(source);
				audioSourcesInUse.RemoveAt(i);
			}
		}
	}

	AudioSource GetAvailableAudioSource()
	{
		if (freeAudioSources.Count > 0)
		{
			var source = freeAudioSources.Pop();
			audioSourcesInUse.Add(source);
			return source;
		}
		else
		{
			var source = audioSourcesInUse[0];
			audioSourcesInUse.RemoveAt(0);
			audioSourcesInUse.Add(source);
			return source;
		}
	}

	void PlayAudioClip(AudioConfiguration audioConfig)
	{
		var source = GetAvailableAudioSource();
		audioConfig.AssignToAudioSource(source);
		source.Play();
	}

	private void OnJump(EventOnRobotJump obj)
	{
		// Only play the sound if it came from the character this Controller refers to
		if (entityView.EntityRef.Equals(obj.Robot))
			PlayAudioClip(jumpAudioClip);
	}

	private void OnDoubleJump(EventOnRobotDoubleJump obj)
	{
		if (entityView.EntityRef.Equals(obj.Robot))
			PlayAudioClip(doubleJumpAudioClip);
	}

	private void OnLanding(EventOnRobotGrounded obj)
	{
		if (entityView.EntityRef.Equals(obj.Robot))
			PlayAudioClip(landingAudioClip);
	}

	private void OnDeath(EventOnRobotDeath obj)
	{
		if (entityView.EntityRef.Equals(obj.Robot))
			PlayAudioClip(deathClip);
	}

	private void OnRespawn(EventOnRobotRespawn obj)
	{
		if (entityView.EntityRef.Equals(obj.Robot))
			PlayAudioClip(respawnClip);
	}

	// Triggered via animation events
	public void OnFootStep()
	{
		PlayAudioClip(footstepsClip);
	}
}