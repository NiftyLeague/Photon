using UnityEngine;
using System.Collections;
using Quantum;

public sealed class BulletTrailFx : BulletFx
{
  public float duration = 1.0f;
  public float destroyDelay = 0.2f;
  public TrailRenderer bulletTrail;
  public ParticleSystem bulletParticle;

  private void Awake()
  {
    bulletTrail.enabled = false; bulletTrail.Clear();
  }

  public unsafe override void OnFx(Quantum.EntityRef robotRef, Vector3 position, Vector3 direction)
  {
    if (robotRef == EntityRef.None)
    {
      return;
    }
    var f = QuantumRunner.Default.Game.Frames.Verified;
    var weaponInventory = f.Get<WeaponInventory>(robotRef);
    var weapon = weaponInventory.Weapons[weaponInventory.CurrentWeaponIndex];
    var weaponData = f.FindAsset<WeaponData>(weapon.WeaponData.Id);

    var player = f.Get<PlayerID>(robotRef);
    var robotMovement = f.Get<Movement>(robotRef);
    var robotTransform = f.Get<Transform2D>(robotRef);


    var fireSpotOffset = WeaponHelper.GetFireSpotWorldOffset(
      weaponData,
      f.GetPlayerInput(player.PlayerRef)->AimDirection,
      robotMovement.IsFacingRight
    );

    bulletTrail.transform.position = robotTransform.Position.ToUnityVector3() + fireSpotOffset.ToUnityVector3();
    bulletParticle.transform.position = robotTransform.Position.ToUnityVector3() + fireSpotOffset.ToUnityVector3();
    bulletTrail.Clear();
    StartCoroutine(MakeEffect(robotTransform.Position.ToUnityVector3() + fireSpotOffset.ToUnityVector3(), position));
  }

  IEnumerator MakeEffect(Vector3 startPosition, Vector3 finalPosition)
  {
    bulletTrail.Clear();
    bulletParticle.Simulate(0);
    bulletParticle.Play();
    float t = 0;
    while (t < duration)
    {
      t += Time.deltaTime;
      bulletTrail.transform.position = Vector3.Lerp(startPosition, finalPosition, t / duration);
      bulletParticle.transform.position = Vector3.Lerp(startPosition, finalPosition, t / duration);
      yield return null;
      bulletTrail.enabled = true;
    }
    bulletTrail.transform.position = finalPosition;
    bulletParticle.transform.position = finalPosition;
    Destroy(gameObject, destroyDelay);
  }
}
