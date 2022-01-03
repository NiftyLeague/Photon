using Quantum;
using Photon.Deterministic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public unsafe class UnparentTrailOnDestroy : MonoBehaviour
{

  public GameObject effect;
  public EntityView bulletView;
  private EntityRef robotEntityRef;
  private Transform parent;

  private void Start()
  {
    var f = QuantumRunner.Default.Game.Frames.Verified;
    var bulletFields = f.Get<BulletFields>(bulletView.EntityRef);

    robotEntityRef = bulletFields.Source;
    QuantumEvent.Subscribe<EventOnBulletDestroyed>(this, OnBulletDestroyed);
    parent = transform.parent;
    transform.parent = null;

    var weaponInventory = f.Get<WeaponInventory>(robotEntityRef);
    var weapon = weaponInventory.Weapons[weaponInventory.CurrentWeaponIndex];
    var weaponData = f.FindAsset<WeaponData>(weapon.WeaponData.Id);

    var playerID = f.Get<PlayerID>(robotEntityRef);
    var robotMovement = f.Get<Movement>(robotEntityRef);
    var robotTransform = f.Get<Transform2D>(robotEntityRef);

    var fireSpotWorldOffset = WeaponHelper.GetFireSpotWorldOffset(
        weaponData,
         f.GetPlayerInput(playerID.PlayerRef)->AimDirection,
         robotMovement.IsFacingRight
     );

    transform.position = (robotTransform.Position + fireSpotWorldOffset).ToUnityVector3();
  }
  private void Update()
  {
    if (parent != null)
    {
      transform.position = parent.position;
    }
  }
  private void OnBulletDestroyed(EventOnBulletDestroyed eventData)
  {
    Debug.Log("OnBulletDestroyed trail");
    if (!robotEntityRef.Equals(eventData.Robot))
      return;
    enabled = false;

    Debug.Log("OnBulletDestroyed trail2");

    Vector3 position = eventData.BulletPosition.ToUnityVector3();
    Vector3 direction = eventData.BulletDirection.ToUnityVector3();
    effect.transform.position = position;
  }

  private void OnDestroy()
  {
    QuantumEvent.UnsubscribeListener(this);
  }

  public static FPVector2 GetFireSpotWorldOffset(WeaponData weaponData, FP angle, bool isFacingRight)
  {
    var positionOffset = weaponData.PositionOffset;
    var firespotVector = weaponData.FireSpotOffset;

    if (!isFacingRight)
    {
      positionOffset.X = -positionOffset.X;
      firespotVector.Y = -firespotVector.Y;
    }

    return positionOffset + FPVector2.Rotate(firespotVector, angle);
  }
}
