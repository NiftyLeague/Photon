using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueless/Inventory", fileName = "DefaultInventory.asset")]
public class ItemInventory : ScriptableObject
{
	private static ItemInventory _instance;
	public static ItemInventory Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load<ItemInventory>("DefaultInventory");
			}
			return _instance;
		}
	}

	public List<InventoryItem> items;

	public T Find<T>(string id) where T : InventoryItem
	{
		return items.OfType<T>().FirstOrDefault(x => x.itemId == id);
	}
}