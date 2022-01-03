using System.Collections.Generic;
using Quantum;
using UnityEngine;

public class FloatingCombatText : MonoBehaviour
{
  [Header("References")]
  public Canvas canvas;
  public RectTransform canvasTransform;
  public FloatingCombatTextEntry entryPrefab;

  private Queue<FloatingCombatTextEntry> entryPool = new Queue<FloatingCombatTextEntry>();

  private void Start()
  {
    SpawnEntries(16);
    QuantumEvent.Subscribe<EventOnRobotTakeDamage>(this, OnRobotTookDamage);
  }

  private void OnDestroy()
  {
    QuantumEvent.UnsubscribeListener(this);
  }

  private unsafe void OnRobotTookDamage(EventOnRobotTakeDamage eventData)
  {
    var f = QuantumRunner.Default.Game.Frames.Verified;
    var robot = f.Get<PlayerID>(eventData.Robot);
    var sourceRobot = f.Get<PlayerID>(eventData.Source);

    if (!QuantumRunner.Default.Game.PlayerIsLocal(robot.PlayerRef) && !QuantumRunner.Default.Game.PlayerIsLocal(sourceRobot.PlayerRef))
    {
      return;
    }
    if (entryPool.Count == 0)
    {
      SpawnEntries(16);
    }


    var entry = entryPool.Dequeue();

    var robotPosition = f.Get<Transform2D>(eventData.Robot).Position;
    var sourcePosition = f.Get<Transform2D>(eventData.Source).Position;

    Vector3 viewportPos = canvas.worldCamera.WorldToViewportPoint(robotPosition.ToUnityVector3());
    var direction = (robotPosition - sourcePosition).ToUnityVector2();

    // If visible, update the position
    float width = canvasTransform.sizeDelta.x;
    float height = canvasTransform.sizeDelta.y;
    var pos = new Vector3(width * viewportPos.x - width / 2, height * viewportPos.y - height / 2);

    viewportPos = pos;
    viewportPos.x = Mathf.FloorToInt(viewportPos.x);
    viewportPos.y = Mathf.FloorToInt(viewportPos.y);
    viewportPos.z = 0f;

    entry.rectTransform.anchoredPosition = viewportPos;
    entry.Activate(eventData.Damage.AsInt, direction);
  }

  private void SpawnEntries(int amount)
  {
    for (int i = 0; i < amount; i++)
    {
      var entry = Instantiate(entryPrefab, transform);
      entry.onAnimationFinished += ReturnObjectToPool;
      entry.Deactivate();
    }
  }

  private void ReturnObjectToPool(FloatingCombatTextEntry entry)
  {
    entryPool.Enqueue(entry);
  }
}