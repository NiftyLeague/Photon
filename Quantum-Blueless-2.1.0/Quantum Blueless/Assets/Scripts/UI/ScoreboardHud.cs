using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quantum;
using Quantum.Demo;
using UnityEngine.SceneManagement;
using Rewired;

public unsafe class ScoreboardHud : QuantumCallbacks
{
  public ScoreboardEntry entryPrefab;
  public GridLayoutGroup gridLayout;
  public CanvasGroup canvasGroup;
  public float alphaAnimationSpeed = 1.0f;
  public KeyCode showKeyCode = KeyCode.Tab;

  private List<ScoreboardEntry> entries = new List<ScoreboardEntry>();
  private List<EntityRef> sortedRobots = new List<EntityRef>();

  private float targetCanvasGroupAlpha = 0.0f;
  private bool alwaysShow = false;
  private Rewired.Player _player; // The Rewired Player
  private GameManager gameManager;


	private void Start()
  {
    gameManager = FindObjectOfType<GameManager>();

    entries = new List<ScoreboardEntry>();
    sortedRobots = new List<EntityRef>();

    QuantumEvent.Subscribe<EventOnRobotDeath>(this, HandleRobotDeath);
    QuantumEvent.Subscribe<EventOnGameEnded>(this, OnGameEnded);
    QuantumEvent.Subscribe<EventOnRobotCreated>(this, CreateNewRobotEntry);

    // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
    _player = ReInput.players.GetPlayer(0);
  }

  public override void OnGameStart(QuantumGame game)
  {
    LoadRobots();
  }

  private void OnDestroy()
  {
    QuantumEvent.UnsubscribeListener(this);
  }

  public unsafe void CreateNewRobotEntry(EventOnRobotCreated e)
  {
    var entry = Instantiate(entryPrefab, gridLayout.transform, false);
    entry.UpdateForRobot(e.Robot);
    entries.Add(entry);
  }

  public void LoadRobots()
  {
    var f = gameManager.Game.Frames.Verified;
    foreach (var (r, playerId) in f.Unsafe.GetComponentBlockIterator<PlayerID>())
    {
      var entry = Instantiate(entryPrefab, gridLayout.transform, false);
      entry.UpdateForRobot(r);
      entries.Add(entry);
    }
  }

  private void Update()
  {
    canvasGroup.alpha =
      Mathf.MoveTowards(canvasGroup.alpha, targetCanvasGroupAlpha, alphaAnimationSpeed * Time.deltaTime);

    bool show = alwaysShow || UnityEngine.Input.GetKey(showKeyCode) || _player.GetButton("Show Scoreboard");
    targetCanvasGroupAlpha = show ? 1.0f : 0.0f;
    canvasGroup.blocksRaycasts = show;
    canvasGroup.interactable = show;
  }

  private unsafe void HandleRobotDeath(EventOnRobotDeath deathEvent)
  {

    sortedRobots.Clear();

    var f = QuantumRunner.Default.Game.Frames.Verified;
    var robots = f.Filter<Score, Status>();
    while (robots.NextUnsafe(out var robot, out var score, out var status))
      sortedRobots.Add(robot);

    sortedRobots.Sort((a, b) =>
    {
      var ra = f.Get<Score>(a);
      var rb = f.Get<Score>(a);

      if (ra.Kills != rb.Kills)
        return rb.Kills - ra.Kills;

      return ra.Deaths - rb.Deaths;
    });

    for (int i = 0; i < entries.Count; i++)
    {
      entries[i].UpdateForRobot(sortedRobots[i]);
    }
  }

  private unsafe void OnGameEnded(EventOnGameEnded onGameEnded)
  {
    alwaysShow = true;
  }
}