using UnityEngine;

public class UnParentParticleOnDestroy : MonoBehaviour
{
	public ParticleSystem particle;
	public Transform t;

	private void Awake()
	{
		particle.transform.parent = null;
	}

	private void Update()
	{
		if (t != null)
			particle.transform.position = t.transform.position;
		if (t == null)
			particle.Stop();
	}
}