using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
    public Transform rightHand;
    public Transform leftHand;
    public Transform lookDir;

    public ParticleSystem muzzle;

    public void ShootFx()
    {
        muzzle.Stop();
        muzzle.Play();
    }
}
