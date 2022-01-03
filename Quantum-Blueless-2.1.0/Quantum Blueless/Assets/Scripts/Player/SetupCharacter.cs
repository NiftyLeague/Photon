using System.Security.Cryptography.X509Certificates;
using UnityEngine;
//using UnityEngine.PostProcessing;
using Quantum;

public unsafe class SetupCharacter : QuantumCallbacks
{
	[System.Serializable]
	public class CharacterView
	{
		public string name;
		public Animator animator;
		public IkControl ikControl;
		public Transform SHRoot;
		public PlayerModelEditor modelEditor;
	}

	public PlayerAnimatorObserver playerAnimationObserver;
	public WeaponController weaponController;
	public CharacterView[] charactersConfig;
	public WeaponAnimationRoot weaponAnimationRoot;

	public InventoryBody.BodyType currentCharacter;

	private PlayerBlink blinkComponent;
	private EntityView entityView;
	private GameManager gameManager;

	protected override void OnEnable()
	{
		base.OnEnable();
		gameManager = FindObjectOfType<GameManager>();
		entityView = GetComponentInParent<EntityView>();
	}


	private void Start()
	{
		blinkComponent = GetComponent<PlayerBlink>();
    var f = gameManager.Game.Frames.Verified;

    var playerID = f.Get<PlayerID>(entityView.EntityRef);
    var player = f.GetPlayerData(playerID.PlayerRef);

    var item = ItemInventory.Instance.Find<InventoryBody>(player.BodyId);
    currentCharacter = (item == null) ? InventoryBody.BodyType.Kara : item.bodyType;

    charactersConfig[(int)currentCharacter].modelEditor.Setup(item);
    UpdateChar();
  }

	private void UpdateChar()
	{
		weaponController.ik = charactersConfig[( int ) currentCharacter].ikControl;
		playerAnimationObserver.animator = charactersConfig[( int ) currentCharacter].animator;

		for( int i = 0; i < charactersConfig.Length; i++ )
		{
			if( i == ( int ) currentCharacter )
			{
				charactersConfig[i].ikControl.gameObject.SetActive( true );
			}
			else
			{
				charactersConfig[i].ikControl.gameObject.SetActive( false );
			}
		}

		weaponAnimationRoot.SHRoot = charactersConfig[( int ) currentCharacter].SHRoot;
		blinkComponent.SetupMaterials();
	}
}