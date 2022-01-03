using System.Collections;
using System.Collections.Generic;
using Quantum;
using TMPro;
using UnityEngine;

public class QuantaWidget : MonoBehaviour
{
	public TMP_Text coinAmount;
	public SfxController.AudioConfiguration iapClick;
	
	void Start()
	{
		PlayerData.OnPlayerDataReceived += SetupCallbacks;
		SetupCallbacks();
	}

	public void OnClick()
	{
		//LobbyAudio.Instance.PlayAudioClip(iapClick);
	}
	
	private void SetupCallbacks()
	{
		if (PlayerData.Instance == null)
		{
			gameObject.SetActive(false);
		}
		else
		{
			gameObject.SetActive(true);
			PlayerData.Instance.onPlayerDataUpdated += UpdateCoins;
		}
	}

	private void UpdateCoins()
	{
		coinAmount.text = PlayerData.Instance.coins.ToString();
	}
}
