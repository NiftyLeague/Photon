using System.Collections.Generic;
using Quantum;
using UnityEngine;

/// <summary>
/// This Behavior handles events not related to player actions that require audio feedback, such as shooting and explosions
///
/// Uses the default Unity Audio API for simplicity
/// </summary>
public class SfxController : MonoBehaviour
{
  [System.Serializable]
  public struct AudioConfiguration
  {
    public AudioClip clip;

    [Range(0, 1.0f)]
    public float volume;

    public bool is2D;

    public bool loop;

    public float delay;

    public string Name {
      get { return clip == null ? "No clip selected" : clip.name; }
    }

    public bool IsValid()
    {
      return clip != null;
    }

    public void AssignToAudioSource(AudioSource audioSource)
    {
      audioSource.volume = volume;
      audioSource.clip = clip;
      audioSource.spatialBlend = is2D ? 0.0f : 1.0f;
      audioSource.loop = loop;
    }
  }

  [Header("References")]
  public AudioSource audioSourcePrefab;

  [Header("Configurations")]
  public int maxAudioSources = 8;
  public Transform audioSourceDefaultParent;

  [Header("Audios")]
  public AudioConfiguration playerHitAudio;
  public AudioConfiguration playerKillAudio;
  public AudioConfiguration playerDamageTakenAudio;
  public AudioConfiguration skillCastingAudio;
  public AudioConfiguration skillActivationAudio;
  public AudioConfiguration timeWarningAudio;

  private readonly Stack<AudioSource> freeAudioSources = new Stack<AudioSource>();
  private List<AudioSource> audioSourcesInUse = new List<AudioSource>();
  private GameManager gameManager;

  private void Start()
  {
    gameManager = FindObjectOfType<GameManager>();

    for (int i = 0; i < maxAudioSources; i++)
    {
      var audioSource = Instantiate(audioSourcePrefab, audioSourceDefaultParent);
      audioSource.transform.localPosition = Vector3.zero;

      freeAudioSources.Push(audioSource);
    }

    RegisterCallbacks();
  }

  private void Update()
  {
    for (var i = audioSourcesInUse.Count - 1; i >= 0; i--)
    {
      var source = audioSourcesInUse[i];
      if (!source.isPlaying)
      {
        freeAudioSources.Push(source);
        audioSourcesInUse.RemoveAt(i);
        source.transform.SetParent(audioSourceDefaultParent);
        source.transform.position = Vector3.zero;
      }
    }
  }

  private void OnDestroy()
  {
    UnregisterCallbacks();
  }

  private void RegisterCallbacks()
  {
    QuantumEvent.Subscribe<EventOnRobotTakeDamage>(this, OnRobotDamage);
    QuantumEvent.Subscribe<EventOnWeaponShoot>(this, OnWeaponShot);
    QuantumEvent.Subscribe<EventOnBulletDestroyed>(this, OnBulletDestroyed);
    QuantumEvent.Subscribe<EventOnSkillCasted>(this, OnSkillCasted);
    QuantumEvent.Subscribe<EventOnSkillActivated>(this, OnSkillActivated);
    QuantumEvent.Subscribe<EventOnRobotDeath>(this, OnRobotDeath);
  }

  private void UnregisterCallbacks()
  {
    QuantumEvent.UnsubscribeListener(this);
  }

  AudioSource GetAvailableAudioSource()
  {
    if (freeAudioSources.Count > 0)
    {
      var source = freeAudioSources.Pop();
      audioSourcesInUse.Add(source);
      return source;
    }
    else
    {
      var source = audioSourcesInUse[0];
      audioSourcesInUse.RemoveAt(0);
      audioSourcesInUse.Add(source);
      return source;
    }
  }

  void PlayAudioClip(AudioConfiguration audioConfig)
  {
    var source = GetAvailableAudioSource();
    audioConfig.AssignToAudioSource(source);

    source.transform.position = Vector3.zero;
    source.Play();
  }

  void PlayAudioClip(AudioConfiguration audioConfig, Transform parent)
  {
    var source = GetAvailableAudioSource();
    audioConfig.AssignToAudioSource(source);

    source.transform.SetParent(parent);
    source.transform.localPosition = Vector3.zero;
    source.Play();
  }

  void PlayAudioClip(AudioConfiguration audioConfig, Vector3 position)
  {
    var source = GetAvailableAudioSource();
    audioConfig.AssignToAudioSource(source);

    source.transform.position = position;
    source.Play();
  }

  private unsafe void OnRobotDamage(EventOnRobotTakeDamage eventData)
  {
    var localRef = gameManager.LocalView.EntityRef;
    var targetRobotTransform = QuantumRunner.Default.Game.Frames.Verified.Get<Transform2D>(eventData.Robot);

    if (localRef.Equals(eventData.Robot))
    {
      PlayAudioClip(playerDamageTakenAudio, gameManager.LocalView.transform.position);
    }
    else if (localRef.Equals(eventData.Source))
    {
      PlayAudioClip(playerHitAudio, targetRobotTransform.Position.ToUnityVector3());
    }
  }

  private unsafe void OnWeaponShot(EventOnWeaponShoot eventData)
  {
    var f = QuantumRunner.Default.Game.Frames.Verified;

    var weaponInventory = f.Get<WeaponInventory>(eventData.Robot);
    var weapon = weaponInventory.Weapons[weaponInventory.CurrentWeaponIndex];
    var weaponData = f.FindAsset<WeaponData>(weapon.WeaponData.Id);

    var asset = weaponData.GetUnityAsset();

    var robotView = PlayerView.GetPlayerView(eventData.Robot);

    var robotTransform = f.Get<Transform2D>(eventData.Robot);

    if (robotView != null)
    {
      PlayAudioClip(asset.shootAudio, robotView.transform);
    }
    else
    {
      PlayAudioClip(asset.shootAudio, robotTransform.Position.ToUnityVector3());
    }
  }

  private void OnBulletDestroyed(EventOnBulletDestroyed eventData)
  {
    var asset = UnityDB.FindAsset<BulletDataAsset>(eventData.BulletDataId);

    if (asset.bulletDestroyAudio.IsValid())
      PlayAudioClip(asset.bulletDestroyAudio, eventData.BulletPosition.ToUnityVector3());
  }

  private unsafe void OnSkillCasted(EventOnSkillCasted eventData)
  {
    var f = QuantumRunner.Default.Game.Frames.Verified;
    if (!f.Exists(eventData.Skill)) {
      return;
    }
    var skillFields = f.Get<SkillFields>(eventData.Skill);
    var skillTransform = f.Get<Transform2D>(eventData.Skill);
    var robotView = PlayerView.GetPlayerView(skillFields.Source);

    if (robotView != null)
    {
      PlayAudioClip(skillCastingAudio, robotView.transform);
    }
    else
    {
      PlayAudioClip(skillCastingAudio, skillTransform.Position.ToUnityVector3());
    }
  }

  private void OnSkillActivated(EventOnSkillActivated eventData)
  {
    PlayAudioClip(skillActivationAudio, eventData.SkillPosition.ToUnityVector3());
  }

  private unsafe void OnRobotDeath(EventOnRobotDeath eventData)
  {
    var player = QuantumRunner.Default.Game.Frames.Verified.Get<PlayerID>(eventData.Killer);
    if (QuantumRunner.Default.Game.PlayerIsLocal(player.PlayerRef))
    {
      PlayAudioClip(playerKillAudio);
    }
  }

  public void PlayWarningAudio()
  {
    PlayAudioClip(timeWarningAudio);
  }
}
