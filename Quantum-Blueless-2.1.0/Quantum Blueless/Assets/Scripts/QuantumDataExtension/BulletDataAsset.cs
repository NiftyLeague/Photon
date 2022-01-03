using UnityEngine;

/// <summary>
/// Having this allows us to have unity-specific asset linking
/// </summary>
public partial class BulletDataAsset
{
	[Header( "View Configuration", order = 9 )]
	public BulletFx bulletDestroyFx;
	public SfxController.AudioConfiguration bulletDestroyAudio;
}
