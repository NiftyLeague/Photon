using Quantum;
using UnityEngine;
using System.Collections;

public unsafe class SkillView : MonoBehaviour
{
	public ParticleSystem effectExplosionPrefab;
	public ParticleSystem effectHitPrefab;
	public ParticleSystem effectPrefab;

	private void Start()
	{
    QuantumEvent.Subscribe<EventOnSkillHitTarget>(this, HitEffect);
    QuantumEvent.Subscribe<EventOnSkillActivated>(this, SkillActivated);
	}

	private void OnDestroy()
	{
    QuantumEvent.UnsubscribeListener(this);
	}

	private void SkillActivated(EventOnSkillActivated eventData)
	{
		Instantiate(effectExplosionPrefab, eventData.SkillPosition.ToUnityVector3(), Quaternion.identity);
	}

	private void HitEffect(EventOnSkillHitTarget eventData)
	{
    var robotPosition = QuantumRunner.Default.Game.Frames.Verified.Get<Transform2D>(eventData.Target).Position;
    var initialPosition = eventData.SkillPosition.ToUnityVector3();
    var finalPosition = robotPosition.ToUnityVector3();
    StartCoroutine(HitEffectCoroutine(initialPosition, finalPosition));
  }

	private IEnumerator HitEffectCoroutine(Vector3 initialPosition, Vector3 finalPosition)
	{
		var obj = Instantiate(effectPrefab, initialPosition, Quaternion.identity);
		yield return null;
		obj.transform.position = finalPosition;
		Instantiate(effectHitPrefab, finalPosition, Quaternion.identity);
	}
}