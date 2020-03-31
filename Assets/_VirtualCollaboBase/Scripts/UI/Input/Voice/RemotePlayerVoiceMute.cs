using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerVoiceMute : MonoBehaviour
{

    public bool isMute = true;
    private PhotonView m_PhotonView = null;


    // Start is called before the first frame update
    void Start()
    {
        m_PhotonView = GetComponent<PhotonView>();
        if (m_PhotonView.IsMine)
        {
            //自分の場合はいらないので削除する。
            Destroy(this);
        }
    }

    public void MuteVoice()
    {
        isMute = true;
    }

    public void UnMuteVoice()
    {
        isMute = false;
    }

    /// <summary>
    /// Raises the audio filter read event.
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="channels">Channels.</param>
    void OnAudioFilterRead(float[] data, int channels)
    {

        if (isMute)
        {
            for (int i = 0; i < data.Length; ++i)
                data[i] = data[i] * 0.0f;
        }


    }
}
