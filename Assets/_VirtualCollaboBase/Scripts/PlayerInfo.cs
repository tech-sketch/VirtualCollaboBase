using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerInfo : MonoBehaviour
{
    public Text nameText;
    public RemotePlayerVoiceMute VoiceMute;
    [SerializeField] private PhotonView m_PhotonView = null;
    // Start is called before the first frame update
    void Start()
    {
     
        nameText.text= m_PhotonView.Owner.NickName;
    }
}


