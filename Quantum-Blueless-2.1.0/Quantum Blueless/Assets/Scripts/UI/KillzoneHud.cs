using Quantum;
using UnityEngine;

public class KillzoneHud : MonoBehaviour
{
	public KillzoneHudEntry hudEntry;
	public RectTransform hudEntryLayout;

	void Start()
	{
    QuantumEvent.Subscribe<EventOnRobotDeath>(this, HandleRobotDeath);
	}

	private void OnDestroy()
	{
    QuantumEvent.UnsubscribeListener(this);
  }

  private void HandleRobotDeath(EventOnRobotDeath deathEvent)
	{
		var entry = Instantiate(hudEntry, hudEntryLayout);
		entry.Setup(deathEvent);
		entry.transform.SetAsFirstSibling();
	}
}
