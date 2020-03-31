using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using VRM;

public class LipSyncSetupper : MonoBehaviour
{
    [SerializeField] private GameObject m_PhotonVoice = null;

    private PhotonView m_PhotonView = null;


    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
        SetupLipSync();
    }

    private void Start()
    {
		if( m_PhotonView == null )
		{
			Debug.LogWarning( "LipSyncSetupper PhotonView is null" );
			return;
		}

        // MicWrapperのMicrophone.Start後のこのタイミングが重要
        if (m_PhotonView.IsMine == true)
        {
            AudioSource audioSource = m_PhotonVoice.GetComponent<AudioSource>();
            audioSource.loop = true;     // Set the AudioClip to loop
            audioSource.mute = false;

            audioSource.clip = Photon.Voice.Unity.MicWrapper.mic;
            // Play the audio source
            audioSource.Play();
        }
    }

    private void SetupLipSync()
    {
        VRMBlendShapeProxy vrmBlendShapeProxy = GetComponentInChildren<VRMBlendShapeProxy>();
        VRMLipSyncMorphTarget morph_target = vrmBlendShapeProxy.gameObject.AddComponent<VRMLipSyncMorphTarget>();
        morph_target.blendShapeProxy = vrmBlendShapeProxy;

        OVRLipSyncContext lipsync_context = m_PhotonVoice.GetComponent<OVRLipSyncContext>();
        morph_target.lipsyncContext = lipsync_context;

		if( m_PhotonView == null )
		{
			Debug.LogWarning( "LipSyncSetupper PhotonView is null" );
			return;
		}

        if (m_PhotonView.IsMine == false)
        {
            lipsync_context.audioLoopback = true;
        }
    }
}
