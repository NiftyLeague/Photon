using System.Collections;
using Quantum;
using TMPro;
using UnityEngine;

public class KillzoneHudEntry : MonoBehaviour
{
	[Header( "References" )]
	public TMP_Text entryText;

	public CanvasGroup canvasGroup;

	[Header( "Configurations" )]
	public AnimationCurve alphaCurve;
	public Color killerNameColor = Color.yellow;
	public Color deadNameColor = Color.red;

	public RectTransform groupTransform;
	public AnimationCurve positionCurve;
	public float animateInDuration = 0.5f;

	public float aliveTime = 8.0f;

	public unsafe void Setup( EventOnRobotDeath deathEvent )
	{
    var f = QuantumRunner.Default.Game.Frames.Verified;
    var deadRobot = f.Get<PlayerID>(deathEvent.Robot);
    var deadRobotPlayer = f.GetPlayerData(deadRobot.PlayerRef);

    var killerRobot = f.Get<PlayerID>(deathEvent.Killer);
    var killerRobotPlayer = f.GetPlayerData(killerRobot.PlayerRef);

    StartCoroutine(AnimateIn());

    if (deadRobot.PlayerRef == killerRobot.PlayerRef)
    {
      entryText.text = string.Format("<color={0}>{1}</color> suicided", ColorToHex(deadNameColor), deadRobotPlayer.PlayerName);
    }
    else
    {
      entryText.text = string.Format("<color={0}>{1}</color> killed <color={2}>{3}</color>",
        ColorToHex(killerNameColor),
        killerRobotPlayer.PlayerName,
        ColorToHex(deadNameColor),
        deadRobotPlayer.PlayerName);
    }

    StartCoroutine(AnimateOutAndDestroy());
  }

	private IEnumerator AnimateIn()
	{
		float originalX = groupTransform.anchoredPosition.x;

		for( float t = 0.0f; t < animateInDuration; t += Time.deltaTime )
		{
			float x = originalX * positionCurve.Evaluate( t / animateInDuration );

			Vector3 position = groupTransform.anchoredPosition;
			position.x = x;
			groupTransform.anchoredPosition = position;

			yield return null;
		}

		{
			Vector3 position = groupTransform.anchoredPosition;
			position.x = 0.0f;
			groupTransform.anchoredPosition = position;
		}
	}

	private IEnumerator AnimateOutAndDestroy()
	{
		float originalAlpha = canvasGroup.alpha;

		for( float t = 0; t < aliveTime; t += Time.deltaTime )
		{
			var alpha = originalAlpha * alphaCurve.Evaluate( t / aliveTime );
			canvasGroup.alpha = alpha;

			yield return null;
		}

		Destroy( gameObject );
	}

	private static string ColorToHex( Color32 color )
	{
		return string.Format( "#{0:X2}{1:X2}{2:X2}", color.r, color.g, color.b );
	}
}