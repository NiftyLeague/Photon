using UnityEngine;

[CreateAssetMenu(menuName = "Blueless/Coin Pack", fileName = "coinPack.asset")]
public class CoinPack : ScriptableObject
{
	public int amount;
	public string price;
	public string description;
	
	public bool free;
}