using Photon.Deterministic;
using Quantum;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using QTuple = Photon.Deterministic.QTuple;
using Quantum.Demo;
using Rewired;

public sealed unsafe class LocalInput : QuantumCallbacks
{
  private Vector2 lastPlayerDirection;
  public float aimAssist = 20;
  public float aimSpeed = 2;

  private Rewired.Player _player; // The Rewired Player


  private void Start()
  {
    QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));

    // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
    _player = ReInput.players.GetPlayer(0);
  }

  public void PollInput(CallbackPollInput callback)
  {
    Quantum.Input input = default;
    var player = QuantumRunner.Default.Game.GetLocalPlayers()[0];
    if (UIMain.Client != null && UIMain.Client.InRoom == false && player != 0)
    {
      return;
    }

    var f = QuantumRunner.Default.Game.Frames.Verified;

    EntityRef localRobot = EntityRef.None;

    foreach (var (robot, playerId) in f.Unsafe.GetComponentBlockIterator<PlayerID>())
    {
      var robotPlayerID = f.Get<PlayerID>(robot);
      if (robotPlayerID.PlayerRef == player)
      {
        localRobot = robot;
      }
    }

    input.Fire = _player.GetButton("Fire");

//#if UNITY_MOBILE || UNITY_EDITOR -> to test touch UI
#if UNITY_MOBILE
    var mouseDirection = new Vector2(_player.GetAxis("Mouse Horizontal"), _player.GetAxis("Mouse Vertical"));
    input.Fire |= (mouseDirection.magnitude >= 0.5f);
# endif

    input.Jump = _player.GetButton("Jump");
    input.Movement = (sbyte)(GetMovement() * sbyte.MaxValue).AsInt;
    input.AimDirection = GetAimDirectionForRobot(f, localRobot);
    input.ChangeWeapon = _player.GetButton("Change Weapon");
    input.CastSkill = _player.GetButton("Cast Skill");

    callback.SetInput(input, DeterministicInputFlags.Repeatable);
  }

  private FP GetMovement()
  {
    return FP.FromFloat_UNSAFE(_player.GetAxis("Move Horizontal"));
  }

  private FP GetAimDirectionForRobot(Frame f, EntityRef robot)
  {
    Vector2 direction;

    if (robot == EntityRef.None)
    {
      return 0;
    }

    var robotTransform = f.Get<Transform2D>(robot);

    var isMobile = false;

#if !UNITY_STANDALONE
    isMobile = true;
# endif

    if (isMobile || (UnityEngine.Input.GetJoystickNames().Length != 0 && UnityEngine.Input.GetJoystickNames()[0] != ""))
    {
      var controlDir = new Vector2(_player.GetAxis("Mouse Horizontal"), _player.GetAxis("Mouse Vertical"));
      if (controlDir.sqrMagnitude > 0.1f)
      {
        direction = (Vector2)controlDir;

      }
      else if (Mathf.Abs(GetMovement().AsFloat) > 0.1f)
      {
        direction = new Vector2(GetMovement().AsFloat, 0);
      }
      else
      {
        direction = lastPlayerDirection;
      }
      lastPlayerDirection = direction;

      //AIM ASSIST
      EntityRef localRobot = EntityRef.None;
      var minorAngle = aimAssist;

      foreach (var (r, playerId) in f.Unsafe.GetComponentBlockIterator<PlayerID>())
      {
        var player = f.Get<PlayerID>(r);
        if ((QuantumRunner.Default.Game.PlayerIsLocal(player.PlayerRef)) || player.PlayerRef == 0)
        {
          localRobot = r;
        }
      }

      var localRobotPosition = FPVector2.Zero;
      localRobotPosition = f.Get<Transform2D>(localRobot).Position;

      foreach (var (r, playerId) in f.Unsafe.GetComponentBlockIterator<PlayerID>())
      {
        var position = f.Get<Transform2D>(r).Position;
        if (r == localRobot)
        {
          continue;
        }
        var targetDirection = position - localRobotPosition;
        if (Vector2.Angle(direction, targetDirection.ToUnityVector2()) <= minorAngle)
        {
          direction = Vector2.Lerp(direction, targetDirection.ToUnityVector2(), Time.deltaTime * aimSpeed);
        }
      }
    }
    else
    {
      var localRobotPosition = robotTransform.Position.ToUnityVector3();
      var localRobotScreenPosition = Camera.main.WorldToScreenPoint(localRobotPosition);
      var mousePos = UnityEngine.Input.mousePosition;
      direction = mousePos - localRobotScreenPosition;
    }

    var angle = Mathf.Atan2(direction.y, direction.x);
    angle = Mathf.Repeat((angle + 2 * Mathf.PI), 2 * Mathf.PI);

    return FP.FromFloat_UNSAFE(angle);
  }
}