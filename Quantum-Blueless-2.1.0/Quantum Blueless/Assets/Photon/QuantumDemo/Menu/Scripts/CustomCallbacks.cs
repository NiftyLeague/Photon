using Quantum;
using UnityEngine;

public class CustomCallbacks : QuantumCallbacks {

  public override void OnGameStart(QuantumGame game) {
    foreach (var lp in game.GetLocalPlayers()) {
      Debug.Log("CustomCallbacks - sending player: " + lp);
      game.SendPlayerData(lp, new Quantum.RuntimePlayer { });
    }
  }
}
