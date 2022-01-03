using Photon.Deterministic;
using Quantum.Core;
using System;


namespace Quantum
{
  /// <summary>
  ///   Handles movement and input for all players
  ///   Things this system handles:
  ///   - Robot Movement
  ///   - Robot Jump & Double Jump
  /// </summary>
  public unsafe class MovementSystem : SystemMainThread, ISignalOnGameEnded, IKCCCallbacks2D
  {
    public override void Update(Frame f)
    {
      var robotsFilter = f.Filter<Transform2D, PlayerID, Status, Movement, CharacterController2D>();
      while (robotsFilter.NextUnsafe(out var robot, out var transform, out var player, out var status, out var robotMovement, out var kcc))
      {
        if (status->IsDead == true) {
          continue;
        }

        var input = f.GetPlayerInput(player->PlayerRef);

        FP axisHorizontal = input->Movement;
        axisHorizontal /= sbyte.MaxValue;

        var moveScale = FP._1;

        var newPosition = Move(f, robot, new FPVector2(moveScale * axisHorizontal, 0), this);
        transform->Position += newPosition;

        var movementData = f.FindAsset<MovementData>(robotMovement->MovementData.Id);

        if (!robotMovement->PrevGrounded && robotMovement->VirtualGrounded)
        {
          robotMovement->JumpDelayTimer = movementData.JumpDelay;
          robotMovement->CanDoubleJump = true;
          f.Events.OnRobotGrounded(robot);
        }
        else
        {
          robotMovement->JumpDelayTimer -= f.DeltaTime;
        }
        robotMovement->PrevGrounded = robotMovement->VirtualGrounded;

        if (input->Jump.WasPressed)
        {
          if (kcc->Grounded || robotMovement->JumpDelayTimer > 0)
          {
            kcc->Jump(f as FrameBase);
            f.Events.OnRobotJump(robot);
          }
          else if (robotMovement->CanDoubleJump)
          {
            kcc->Jump(f as FrameBase, true, movementData.SecondJumpImpulse);
            robotMovement->CanDoubleJump = false;

            f.Events.OnRobotDoubleJump(robot);
          }
        }
        UpdateIsFacingRight(f, input, robot);
      }
    }

    private void UpdateIsFacingRight(Frame f, Input* input, EntityRef robot)
    {
      var robotMovement = f.Unsafe.GetPointer<Movement>(robot);
      robotMovement->IsFacingRight = input->AimDirection < FP.Rad_90 || input->AimDirection > FP.Rad_180 + FP.Rad_90;
    }

    public void OnGameEnded(Frame f, GameController* gameController)
    {
      foreach (var (robot, kcc) in f.Unsafe.GetComponentBlockIterator<CharacterController2D>())
      {
        kcc->Velocity = FPVector2.Zero;
      }
      f.SystemDisable<MovementSystem>();
    }

    private FPVector2 Move(Frame f, EntityRef robot, FPVector2 velocity, IKCCCallbacks2D callback = null)
    {

      var t = f.Unsafe.GetPointer<Transform2D>(robot);
      var kcc = f.Unsafe.GetPointer<CharacterController2D>(robot);

      var l = Layers.GetLayerMask("Environment");
      var movementPack = CharacterController2D.ComputeRawMovement(f as FrameBase, robot, t, kcc, velocity, callback, l);
      CheckVistualGrounded(f, movementPack, robot);
      var config = f.FindAsset<CharacterController2DConfig>(kcc->Config.Id);
      ComputeRawSteer(kcc, ref movementPack, f.DeltaTime, config);

      var movement = kcc->Velocity * f.DeltaTime;

      if (movementPack.Penetration > FP.EN3)
      {
        if (movementPack.Penetration > config.MaxPenetration)
        {
          movement += movementPack.Correction;
        }
        else
        {
          movement += movementPack.Correction * f.DeltaTime * config.Acceleration;
        }
      }
      return movement;
    }

    private void CheckVistualGrounded(Frame f, CharacterController2DMovement movementPack, EntityRef robot)
    {
      var robotMovement = f.Unsafe.GetPointer<Movement>(robot);

      if (movementPack.Grounded)
      {
        robotMovement->GroundedFramesCount += 1;
      }
      else
      {
        robotMovement->UngroundedFramesCount += 1;
      }

      if (!movementPack.Grounded && robotMovement->UngroundedFramesCount >= 10)
      {
        robotMovement->VirtualGrounded = false;
        robotMovement->GroundedFramesCount = 0;
        robotMovement->UngroundedFramesCount = 0;
      }

      if (movementPack.Grounded && robotMovement->GroundedFramesCount >= 1)
      {
        robotMovement->VirtualGrounded = true;
        robotMovement->GroundedFramesCount = 0;
        robotMovement->UngroundedFramesCount = 0;
      }
    }

    static void ComputeRawSteer(CharacterController2D* kcc, ref CharacterController2DMovement movementPack, FP deltaTime, CharacterController2DConfig config)
    {
      kcc->Grounded = movementPack.Grounded;

      var maxYSpeed = FP._100;
      var minYSpeed = -FP._100;
      switch (movementPack.Type)
      {
        case CharacterMovementType.FreeFall:
          kcc->Velocity.Y -= config.Gravity.Magnitude * deltaTime;
          if (!config.AirControl || movementPack.Tangent == default(FPVector2))
          {
            kcc->Velocity.X = FPMath.Lerp(kcc->Velocity.X, FP._0, deltaTime * config.Braking);
          }
          else
          {
            kcc->Velocity.X += movementPack.Tangent.X * config.Acceleration * deltaTime;
          }
          break;
        case CharacterMovementType.Horizontal:
          kcc->Velocity.X += movementPack.Tangent.X * config.Acceleration * deltaTime;
          kcc->Velocity.Y += movementPack.Tangent.Y * config.Acceleration * deltaTime;
          if (kcc->Velocity.Y <= 0)
          {

            if (kcc->Velocity.SqrMagnitude > kcc->MaxSpeed * kcc->MaxSpeed)
            {
              kcc->Velocity = kcc->Velocity.Normalized * kcc->MaxSpeed;
            }
          }
          break;
        case CharacterMovementType.SlopeFall:
          kcc->Velocity += movementPack.SlopeTangent * config.Acceleration * deltaTime;
          minYSpeed = -config.MaxSlopeSpeed;
          break;
        case CharacterMovementType.None:
          if (kcc->Velocity != default(FPVector2))
          {
            if (kcc->Velocity.Y <= 0)
            {
              kcc->Velocity = FPVector2.Lerp(kcc->Velocity, default(FPVector2), deltaTime * config.Braking);
            }
            if (kcc->Velocity.SqrMagnitude < FP._0_10)
            {
              kcc->Velocity = default(FPVector2);
            }
          }
          minYSpeed = FP._0;
          break;
      }

      if (movementPack.Type != CharacterMovementType.Horizontal)
      {
        kcc->Velocity.Y = FPMath.Clamp(kcc->Velocity.Y, minYSpeed, maxYSpeed);
        kcc->Velocity.X = FPMath.Clamp(kcc->Velocity.X, -kcc->MaxSpeed, kcc->MaxSpeed);
      }
    }

    public bool OnCharacterCollision2D(FrameBase f, EntityRef character, Physics2D.Hit hit)
    {
      // stops jumps when hitting ceilings
      var kcc = f.Unsafe.GetPointer<CharacterController2D>(character);
      if (hit.Normal.Y.RawValue < 0 && kcc->Velocity.Y.RawValue > 0)
      {
        kcc->Velocity.Y.RawValue = 0;
        kcc->Grounded = false;
      }
      return true;
    }

    public void OnCharacterTrigger2D(FrameBase f, EntityRef character, Physics2D.Hit hit)
    {
    }
  }
}