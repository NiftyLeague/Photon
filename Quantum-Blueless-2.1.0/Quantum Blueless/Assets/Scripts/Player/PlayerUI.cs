using TMPro;
using Quantum;
using UnityEngine;
using Photon.Realtime;

public unsafe class PlayerUI : QuantumCallbacks
{

	[Header("References")]
	public SpriteRenderer health;
	public SpriteRenderer weaponAmmo;
	public SpriteRenderer weaponFireRate;

	public TMP_Text playerName;

	[Header("Configurations")]
	public Color enemyColor;
	public Color weaponColor;

	private float originalHealthWidth;
	private float originalAmmoWidth;
	private GameManager gameManager;
	private EntityView entityView;

	protected override void OnEnable()
	{
		base.OnEnable();
		gameManager = FindObjectOfType<GameManager>();
		entityView = GetComponentInParent<EntityView>();
	}


	private void Start()
	{
		originalHealthWidth = health.size.x;
		originalAmmoWidth = weaponAmmo.size.x;

		var f = gameManager.Game.Frames.Verified;
		var player = f.Get<PlayerID>(entityView.EntityRef).PlayerRef;
		playerName.text = f.GetPlayerData(player).PlayerName;

		if (gameManager.Game.PlayerIsLocal(player) == false)
		{
			health.color = enemyColor;
			playerName.color = enemyColor;
		}
	}

	public override void OnUpdateView(QuantumGame game)
	{
		var f = gameManager.Game.Frames.Verified;

		var weaponInventory = f.Get<WeaponInventory>(entityView.EntityRef);
		var currentWeapon = weaponInventory.Weapons[weaponInventory.CurrentWeaponIndex];
		var weaponData = f.FindAsset<WeaponData>(currentWeapon.WeaponData.Id);

		var status = f.Get<Status>(entityView.EntityRef);
		var statusData = f.FindAsset<StatusData>(status.StatusData.Id);


		var healthRatio = (status.CurrentHealth / statusData.MaxHealth).AsFloat;
		var ammoRatio = (float)currentWeapon.currentAmmo / weaponData.MaxAmmo;
		var fireRateRatio = Mathf.Clamp01((float)currentWeapon.FireRateTimer / weaponData.FireRate.AsFloat);

		health.size = new Vector2(healthRatio * 2, 0.25f);
		weaponAmmo.size = new Vector2(ammoRatio * 2, 0.25f);
		weaponFireRate.size = new Vector2((1 - fireRateRatio) * 2, 0.10f);

		weaponColor.a = currentWeapon.IsRecharging ? 0.5f : 1;
		weaponAmmo.color = weaponColor;
	}
}