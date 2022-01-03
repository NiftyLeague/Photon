using Quantum;
using UnityEngine;

/// <summary>
/// This Behavior handles events related to a player character that should have visual fx, such as landing, double jump and jump
/// </summary>
public class PlayerFxController : MonoBehaviour
{
	public ParticleSystem landingParticle;
	public ParticleSystem doubleJumpParticle;
	public ParticleSystem explosionParticle;
	public ParticleSystem jumpParticle;
	public ParticleSystem respawnParticle;
	public EntityView entityView; 

	public Renderer[] robotMeshes;
	public Renderer[] robotUi;


	public PlayerAnimatorObserver animator;

	void Awake()
	{
    QuantumEvent.Subscribe<EventOnRobotDoubleJump>(this, OnDoubleJump);
    QuantumEvent.Subscribe<EventOnRobotGrounded>(this, OnGrounded);
    QuantumEvent.Subscribe<EventOnRobotDeath>(this, OnDeath);
    QuantumEvent.Subscribe<EventOnRobotRespawn>(this, OnRespawn);
    QuantumEvent.Subscribe<EventOnRobotJump>(this, OnJump);

		robotMeshes = transform.Find("Body").GetComponentsInChildren<Renderer>(true);
		robotUi = transform.Find("UI").GetComponentsInChildren<Renderer>();
	}

	private void OnDestroy()
	{
    QuantumEvent.UnsubscribeListener(this);
	}

	private void OnDoubleJump(EventOnRobotDoubleJump eventData)
	{
		if (!entityView.EntityRef.Equals(eventData.Robot))
			return;
		
		animator.OnDoubleJump();
		doubleJumpParticle.Play();
	}

	private void OnJump(EventOnRobotJump eventData)
	{
		if (!entityView.EntityRef.Equals(eventData.Robot))
			return;

		jumpParticle.Play();
	}

	private void OnGrounded(EventOnRobotGrounded eventData)
	{
		if (!entityView.EntityRef.Equals(eventData.Robot))
			return;
		landingParticle.Play();
	}

	private void OnDeath(EventOnRobotDeath eventData)
	{
		if (!entityView.EntityRef.Equals(eventData.Robot))
			return;

		explosionParticle.Play();
		foreach (Renderer r in robotMeshes)
			r.enabled = false;
		foreach (Renderer r in robotUi)
			r.enabled = false;
	}

	private void OnRespawn(EventOnRobotRespawn eventData)
	{
		if (!entityView.EntityRef.Equals(eventData.Robot))
			return;

		respawnParticle.Play();
		foreach (Renderer r in robotMeshes)
			r.enabled = true;
		foreach (Renderer r in robotUi)
			r.enabled = true;
	}
}