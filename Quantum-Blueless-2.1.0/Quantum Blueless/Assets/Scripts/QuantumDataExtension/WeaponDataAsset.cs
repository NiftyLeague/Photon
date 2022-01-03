using UnityEngine;

/// <summary>
/// Having this allows us to have unity-specific asset linking
/// </summary>
public partial class WeaponDataAsset
{
	[Header( "View Configuration" )]
	public SfxController.AudioConfiguration shootAudio;

	public Sprite uiIcon;

	public WeaponView prefab;
}