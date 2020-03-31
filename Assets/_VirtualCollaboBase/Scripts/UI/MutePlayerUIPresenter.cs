using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRUIParts;

public class MutePlayerUIPresenter : MonoBehaviour
{

    private RemotePlayerVoiceMute voiceMute;
    [SerializeField] private Text text;

    public void SetData(PlayerInfo playerInfo)
    {
        voiceMute = playerInfo.VoiceMute;
        ToggleSwitchController toggleController = gameObject.GetComponentInChildren<ToggleSwitchController>();
        toggleController.isOn = voiceMute.isMute;
        text.text = playerInfo.nameText.text;
    }

    public void MuteOn()
    {
        voiceMute.MuteVoice();
    }

    public void MuteOFF()
    {
        voiceMute.UnMuteVoice();
    }

    
}
