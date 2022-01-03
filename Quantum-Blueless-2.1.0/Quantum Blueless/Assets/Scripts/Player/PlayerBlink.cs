using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quantum;

/// <summary>
/// Makes the player flash when receiving damage 
/// </summary>
public sealed unsafe class PlayerBlink : MonoBehaviour
{
	public EntityView entityView;

	public Material blinkDamageMaterial;

	public Renderer boyMesh;
	public Renderer girlMesh;
	public float time = 0.1f;
	private Material[] originalMaterialsBoy;
	private Material[] originalMaterialsGirl;
	private Coroutine blinkRoutine;
	private Material[] blinkMaterialsBoy;
	private Material[] blinkMaterialsGirl;


	private void Start()
	{
    QuantumEvent.Subscribe<EventOnRobotTakeDamage>(this, RobotBlink);
		SetupMaterials();
	}

	public void SetupMaterials()
	{
		blinkMaterialsBoy = new Material[boyMesh.materials.Length];
		blinkMaterialsGirl = new Material[girlMesh.materials.Length];

		for (int i = 0; i < boyMesh.materials.Length; i++)
		{
			blinkMaterialsBoy[i] = blinkDamageMaterial;
			originalMaterialsBoy = boyMesh.sharedMaterials;
		}

		for (int i = 0; i < girlMesh.materials.Length; i++)
		{
			blinkMaterialsGirl[i] = blinkDamageMaterial;
			originalMaterialsGirl = girlMesh.sharedMaterials;
		}
	}

	void RobotBlink(EventOnRobotTakeDamage eventData)
	{
		if (eventData.Robot.Equals(entityView.EntityRef))
		{
			StartCoroutine("Blink");
		}
	}

	System.Collections.IEnumerator Blink()
	{
		girlMesh.materials = blinkMaterialsGirl;
		boyMesh.materials = blinkMaterialsBoy;
		yield return new WaitForSeconds(time);
		boyMesh.materials = originalMaterialsBoy;
		girlMesh.materials = originalMaterialsGirl;
	}

	private void OnDestroy()
	{
    QuantumEvent.UnsubscribeListener(this);
	}
}