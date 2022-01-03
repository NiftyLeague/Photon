using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryEntry : MonoBehaviour
{
	public Image thumbnailImage;

	public GameObject lockedOverlay;

	private InventoryItem targetItem;
	public Action<InventoryEntry> onClickAction;
	
	public void Setup(PlayerData playerData, InventoryItem item, Action<InventoryEntry> clickAction = null)
	{
		targetItem = item;
		thumbnailImage.sprite = item.thumbnail;
		lockedOverlay.SetActive(!playerData.ownedItems.Contains(item));
		
		onClickAction = clickAction;
	}

	public void OnClick()
	{
		if (onClickAction != null)
			onClickAction(this);
	}

	public T GetTarget<T>() where T : InventoryItem
	{
		return targetItem as T;
	}
}