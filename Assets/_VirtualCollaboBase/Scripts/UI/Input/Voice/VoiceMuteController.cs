using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using Photon.Voice.Unity;

public class VoiceMuteController : MonoBehaviour
{

    [SerializeField] private Recorder recorder;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void MuteVoice()
    {
        recorder.TransmitEnabled = false;
    }
    
    private void UnmuteVoice()
    {
        recorder.TransmitEnabled = true;
    }



}
