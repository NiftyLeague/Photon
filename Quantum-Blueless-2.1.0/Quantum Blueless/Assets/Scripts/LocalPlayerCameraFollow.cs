using Photon.Deterministic;
using Quantum;
using UnityEngine;

public unsafe class LocalPlayerCameraFollow : QuantumCallbacks
{
	public float smoothTime = 0.3f;
	public float maxSpeed = 10.0f;
	public float lookOffset = 10.0f;

	private Vector2 currentVelocity;
	private float originalDistance;

	public float zSmoothTime = 0.5f;
	public float zMaxVelocity = 10.0f;

	private float zVelocity = 0.0f;
	private float zDistance = 0.0f;

	private GameManager gameManager;

	protected override void OnEnable()
	{
		base.OnEnable();
		originalDistance = transform.position.z;
		gameManager = FindObjectOfType<GameManager>();
	}

	public override void OnUpdateView(QuantumGame game)
	{
		base.OnUpdateView(game);
		if (gameManager.LocalView == null) { return; }
		var f = gameManager.Game.Frames.Verified;

		var playerView = gameManager.LocalView.GetComponentInChildren<PlayerView>();

		Vector2 cameraPosition = transform.position;
		Vector2 targetPosition = playerView.transform.position;
		targetPosition.x += lookOffset * playerView.lookDirection;
		cameraPosition = Vector2.SmoothDamp(cameraPosition, targetPosition, ref currentVelocity, smoothTime, maxSpeed, Time.deltaTime);

		var targetDistance = 0.0f;
		var weaponInventory = f.Get<WeaponInventory>(gameManager.LocalView.EntityRef);
		var weapon = weaponInventory.Weapons[weaponInventory.CurrentWeaponIndex];
		var weaponData = f.FindAsset<WeaponData>(weapon.WeaponData.Id);

		if (weaponData.IsChargeWeapon)
		{
			var cameraDistance = weaponData.chargeCameraDistance * weapon.chargeTime / weaponData.chargeDuration;
			targetDistance = (float)cameraDistance;
		}

		zDistance = Mathf.SmoothDamp(zDistance, targetDistance, ref zVelocity, zSmoothTime);

		transform.position = new Vector3(
			cameraPosition.x,
			cameraPosition.y,
			originalDistance - zDistance
		);

	}
}
