using Photon.Deterministic;
using System;

namespace Quantum
{
  /// <summary>
  /// Polymorphic data asset for bullets
  /// </summary>
  public abstract partial class BulletData
  {
    public AssetRefEntityPrototype BulletPrototype;
    public Shape2DConfig ShapeConfig;
    public bool bounce;
    public FP Damage;
    public FP Range;
    public FP Duration;
    public FP Torque;
    public FP NegativeGravity;

    /// <summary>
    /// Post hit effects should happen on this function call.
    /// 
    /// Eg.: Explosions, damage calculations, etc.
    /// </summary>
    /// <param name="frame"></param>
    /// <param name="bullet">The bullet that triggered the function call</param>
    /// <param name="targetRobot">The target of the bullet (is null when hitting a static collider)</param>
    public virtual unsafe void BulletAction(Frame frame, EntityRef bullet, EntityRef targetRobot)
    {
    }
  }
}