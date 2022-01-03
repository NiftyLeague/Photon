using Photon.Deterministic;
using Quantum;
using UnityEngine;
using Animator = UnityEngine.Animator;

public sealed unsafe class WeaponController : QuantumCallbacks
{
	[System.NonSerialized]
	public int ammo;
	[System.NonSerialized]
	public float angle;
	public WeaponAnimationRoot animationRoot;

	public Animator animator;
	public IkControl ik;
	public float x = 0.1f;

	private WeaponView[] weapons;
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
		QuantumEvent.Subscribe<EventOnWeaponShoot>(this, ShootEffect);
		QuantumEvent.Subscribe<EventOnRobotChangeWeapon>(this, ChangeWeapon);

		var f = gameManager.Game.Frames.Verified;
		var weaponInventory = f.Get<WeaponInventory>(entityView.EntityRef);

		weapons = new WeaponView[weaponInventory.Weapons.Length];
		for (int i = 0; i < weaponInventory.WeaponsData.Length; i++)
		{
			var weaponData = UnityDB.FindAsset<WeaponDataAsset>(weaponInventory.WeaponsData[i].Id);
			weapons[i] = Instantiate(weaponData.prefab, transform);
			weapons[i].transform.localPosition = Vector3.zero;
			weapons[i].transform.localRotation = Quaternion.identity;
		}
		UpdateWeapon(entityView.EntityRef);
	}

	public override void OnUpdateView(QuantumGame game)
	{
		var f = gameManager.Game.Frames.Verified;
		if (f.Global->GameController.State == GameState.Ended)
		{
			return;
		}

		var player = f.Get<PlayerID>(entityView.EntityRef).PlayerRef;
		var zAngle = f.GetPlayerInput(player)->AimDirection;
		angle = Mathf.Rad2Deg * zAngle.AsFloat;

		var weaponInventory = f.Get<WeaponInventory>(entityView.EntityRef);
		var weapon = weaponInventory.Weapons[weaponInventory.CurrentWeaponIndex];
		var weaponData = f.FindAsset<WeaponData>(weapon.WeaponData.Id);


		ammo = weapon.currentAmmo;
		var position = weaponData.PositionOffset.ToUnityVector3();

		var positionOffset = weaponData.PositionOffset;
		var finalRotation = Quaternion.Euler(Mathf.Rad2Deg * zAngle.AsFloat * -1, 0, 0);
		UpdateWeapon(entityView.EntityRef);

		var robotMovement = f.Get<Movement>(entityView.EntityRef);
		if (!robotMovement.IsFacingRight) finalRotation = Quaternion.Euler(180 - Mathf.Rad2Deg * zAngle.AsFloat * -1, 0, 0);

		transform.localPosition =
			new Vector3(x, positionOffset.Y.AsFloat, positionOffset.X.AsFloat) + animationRoot.weaponOffset;
		transform.localRotation = finalRotation;
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			return;
		}

		var f = gameManager.Game.Frames.Verified;

		var playerID = f.Get<PlayerID>(entityView.EntityRef);
		var weaponInventory = f.Get<WeaponInventory>(entityView.EntityRef);

		var angle = f.GetPlayerInput(playerID.PlayerRef)->AimDirection;
		var weapon = weaponInventory.Weapons[weaponInventory.CurrentWeaponIndex];
		var weaponData = f.FindAsset<WeaponData>(weapon.WeaponData.Id);

		var robotMovement = f.Get<Movement>(entityView.EntityRef);

		var fireSpotOffset = WeaponHelper.GetFireSpotWorldOffset(
			weaponData,
			angle,
			robotMovement.IsFacingRight
		);

		var weaponFireSpotPosition = transform.position + fireSpotOffset.ToUnityVector3();
		var fireDirection = FPVector2.Rotate(FPVector2.Right * FP._2, angle);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, 0.2f);
		Gizmos.DrawWireSphere(weaponFireSpotPosition, 0.2f);
		Gizmos.DrawLine(transform.position, weaponFireSpotPosition);

		Gizmos.color = Color.blue;
		Gizmos.DrawRay(weaponFireSpotPosition, fireDirection.ToUnityVector3());
	}

	private void ChangeWeapon(EventOnRobotChangeWeapon eventData)
	{
		if (eventData.Robot.Equals(entityView.EntityRef))
		{
			animator.SetTrigger("ChangeWeapon");
			UpdateWeapon(eventData.Robot);
		}
	}

	private void UpdateWeapon(EntityRef robot)
	{
		var f = gameManager.Game.Frames.Verified;
		var currentWeaponIndex = f.Get<WeaponInventory>(robot).CurrentWeaponIndex;

		if(weapons == null) { return; }

		for (var i = 0; i < weapons.Length; i++)
		{
			if (i == currentWeaponIndex)
			{
				weapons[i].gameObject.SetActive(true);
				ik.rightHandObj = weapons[i].rightHand;
				ik.leftHandObj = weapons[i].leftHand;
				ik.lookObj = weapons[i].lookDir;
			}
			else
			{
				weapons[i].gameObject.SetActive(false);
			}
		}
	}

	private void OnDestroy()
	{
		QuantumEvent.UnsubscribeListener(this);
	}

	private void ShootEffect(EventOnWeaponShoot eventData)
	{
		var f = gameManager.Game.Frames.Verified;
		var robotInventoty = f.Get<WeaponInventory>(eventData.Robot);

		if (eventData.Robot.Equals(entityView.EntityRef))
		{
			weapons[robotInventoty.CurrentWeaponIndex].ShootFx();
		}
	}
}