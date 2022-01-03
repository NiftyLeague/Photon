using UnityEngine;

[CreateAssetMenu(menuName = "Blueless/Inventory Body", fileName = "body.asset")]
public class InventoryBody : InventoryItem
{
	public enum BodyType
	{
		Will,
		Kara
	}

	public BodyType bodyType;
	public Material bodyMaterial;
	public Material hairMaterial;
}