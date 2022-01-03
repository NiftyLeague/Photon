using System.Linq;
using UnityEngine;
using Quantum;
using Quantum.Demo;

public sealed class AddLocalPlayers : QuantumCallbacks
{
  public AssetRefEntityPrototype Prototype; 
  public override void OnGameStart(QuantumGame game)
	{
    Application.targetFrameRate = 60;
    SendLocalPlayers(game);
  }

  private void SendLocalPlayers(QuantumGame game) {
    var localPlayers = game.GetLocalPlayers();
    for (var i = 0; i < localPlayers.Length; ++i)
    {
      var data = new Quantum.RuntimePlayer();
      
      if (UIMain.Client != null &&  UIMain.Client.IsConnected)
      {
        data.PlayerName = UIMain.Client.LocalPlayer.NickName;
      }

      var playerData = PlayerData.Instance;
      if (playerData != null)
      {
        data.BodyId = playerData.equipment.body.itemId;
      }
      else
      {
        data.BodyId = "green_girl";
      }
      data.PrototypeRef = Prototype;
      game.SendPlayerData(localPlayers[i], data);
    }
  }
}