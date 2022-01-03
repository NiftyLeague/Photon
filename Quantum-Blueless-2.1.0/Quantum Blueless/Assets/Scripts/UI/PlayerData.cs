using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueless/Player Data", fileName = "data.asset")]
public class PlayerData : ScriptableObject
{
	private static PlayerData _instance;
	public static PlayerData Instance
	{
		get { return _instance; }
		set
		{
			_instance = value;
			if (OnPlayerDataReceived != null)
				OnPlayerDataReceived();
		}
	}

	public static Action OnPlayerDataReceived;
	
	[Serializable]
	public class Equipment
	{
		public InventoryBody body;
		public InventoryWeapon firstGun;
		public InventoryWeapon secondGun;
	}

	public string playerName = "";
	public int coins;
	public Equipment equipment = new Equipment();
	public List<InventoryItem> ownedItems;
	
	public Action onPlayerDataUpdated;

	public void TriggerUpdate()
	{
		if (onPlayerDataUpdated != null)
			onPlayerDataUpdated();
	}
}