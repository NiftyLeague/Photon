using UnityEngine;
using Quantum;

public sealed unsafe class PlayerAnimatorObserver : QuantumCallbacks
{
  public Animator animator;
  public Vector3 velocity;

  private Vector3 lastPosition;

  private GameManager gameManager;
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
    var robotMovement = f.Get<Movement>(entityView.EntityRef);

    velocity = (transform.position - lastPosition) / Time.deltaTime;
    lastPosition = transform.position;

    animator.SetBool("IsFacingRight", robotMovement.IsFacingRight);
    animator.SetBool("IsGrounded", robotMovement.VirtualGrounded);
    var vel = velocity.x;
    if (!robotMovement.IsFacingRight)
    {
      vel *= -1;
    }
    animator.SetFloat("VelocityX", vel);
  }

  public void OnDoubleJump()
  {
    animator.SetTrigger("DoubleJump");
  }
}