using Photon.Deterministic;

namespace Quantum
{
  public partial class WeaponData
  {
    public FP FireRate;
    public FP ShootForce;
    public int MaxAmmo;
    public FP RechargeTimer;
    public FP TimeToRecharge;

    public FP chargeDuration;
    public FP chargeCameraDistance;

    public FP aimMovementScale = FP._1;

    public FPVector2 FireSpotOffset;
    public FPVector2 PositionOffset;

    public AssetRefBulletData BulletData;

    public bool IsChargeWeapon {
      get { return chargeDuration > FP._0; }
    }
  }
}