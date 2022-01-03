using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateIfMobile : MonoBehaviour 
{ 
    void Awake() {
        if (Application.isMobilePlatform)
            gameObject.SetActive(false);
    }
}
