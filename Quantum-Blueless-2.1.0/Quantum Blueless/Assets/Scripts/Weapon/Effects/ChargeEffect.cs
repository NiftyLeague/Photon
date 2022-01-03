using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quantum;

public unsafe class ChargeEffect : MonoBehaviour
{
	private EntityView entityView;
	
	public ParticleSystem chargeEffect;
	public AudioSource audioSource;
	
	public SfxController.AudioConfiguration chargeInAudio;
	public SfxController.AudioConfiguration chargeLoopAudio;

	private void Start()
	{
		entityView = transform.root.GetComponent<EntityView>();
	}

	// Update is called once per frame
	void Update()
	{
    var weaponInventory = QuantumRunner.Default.Game.Frames.Verified.Get<WeaponInventory>(entityView.EntityRef);

    var chargeTime = weaponInventory.Weapons[weaponInventory.CurrentWeaponIndex].chargeTime;

    if (chargeTime > 0)
    {
      if (!chargeEffect.isPlaying)
        chargeEffect.Play();

      if (!audioSource.isPlaying)
      {
        chargeInAudio.AssignToAudioSource(audioSource);
        audioSource.Play();
        StartCoroutine(PlayChargeLoop());
      }
    }
    else
    {
      chargeEffect.Stop();
      audioSource.Stop();
      StopAllCoroutines();
    }
  }

	private IEnumerator PlayChargeLoop()
	{
		yield return new WaitForSeconds(chargeInAudio.clip.length - 0.1f);
		chargeLoopAudio.AssignToAudioSource(audioSource);
		audioSource.Play();
	}
}