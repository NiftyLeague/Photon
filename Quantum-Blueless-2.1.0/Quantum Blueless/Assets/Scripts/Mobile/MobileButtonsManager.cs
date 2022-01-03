using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileButtonsManager : MonoBehaviour
{
  public bool ShowOnEditor = false;

  void Start()
  {
#if UNITY_STANDALONE
#if UNITY_EDITOR
    if (ShowOnEditor == false)
    {
      gameObject.SetActive(false);
    }
#endif
    gameObject.SetActive(false);
#endif
  }
}
