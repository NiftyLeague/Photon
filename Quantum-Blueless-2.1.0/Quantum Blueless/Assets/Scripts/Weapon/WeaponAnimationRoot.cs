using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationRoot : MonoBehaviour 
{
    public Transform SHRoot;
    public Vector3 weaponOffset;
   	
    private float minShRoot = 0.7f;
	
	void Update ()
    {
        weaponOffset = new Vector3(0, SHRoot.localPosition.y - minShRoot, 0);
    }
}
