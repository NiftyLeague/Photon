using Photon.Deterministic;

namespace Quantum
{
  public static unsafe class WeaponHelper
  {
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
}
