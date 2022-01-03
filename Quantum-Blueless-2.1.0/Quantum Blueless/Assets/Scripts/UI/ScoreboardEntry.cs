using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Quantum;

public sealed class ScoreboardEntry : MonoBehaviour
{
	public Image selfBackground;
	public TextMeshProUGUI playerName;
	public TextMeshProUGUI playerKills;
	public TextMeshProUGUI playerDeaths;

	private GameManager gameManager;

	private void Awake()
	{
		gameManager = FindObjectOfType<GameManager>();
	}

	public unsafe void UpdateForRobot(EntityRef robot)
	{
		var f = gameManager.Game.Frames.Verified;
		if(f.Exists(robot) == false) { return; }

		var Id = f.Get<PlayerID>(robot);
		var data = f.GetPlayerData(Id.PlayerRef);
		var score = f.Get<Score>(robot);

		selfBackground.enabled = gameManager.Game.PlayerIsLocal(Id.PlayerRef);

		playerName.text = data.PlayerName;
		playerKills.text = score.Kills.ToString();
		playerDeaths.text = score.Deaths.ToString();
	}
}
