using UnityEngine;

public class PlayerModelEditor : MonoBehaviour
{
	public SkinnedMeshRenderer bodyRenderer;

	public void Setup(string itemIdentifier)
	{
		Setup(ItemInventory.Instance.Find<InventoryBody>(itemIdentifier));
	}

	public void Setup(InventoryBody item)
	{
		if (item != null)
		{
			bodyRenderer.sharedMaterials = new[] {item.hairMaterial, item.bodyMaterial};
		}
	}
}
