using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using Photon.Pun;
using MecanimIKPlus;

public class EmotePlayer : MonoBehaviour
{
	private Animator m_Animator = null;
	private IK_Head_Linkage_CS m_IK_Head = null;
    private IK_CS m_IK_Arm = null;
    private float m_PositionWeight = 0f;
	private float m_PositionWeightR = 0f;
	private float m_RotationWeightR = 0f;
	private float m_PositionWeightL = 0f;
	private float m_RotationWeightL = 0f;
	// Coroutineのキャンセル用
	private int m_CurrentSeqNo = 0;
	private PhotonView m_PhotonView = null;

	[Inject]
	void Initialize( IPlayerAvatarPresenter player_presenter )
	{
		player_presenter.OnLoadModelCompleted
			.Subscribe( go =>
			{
				m_Animator = go.GetComponent<Animator>();
                m_IK_Head = go.GetComponent<IK_Head_Linkage_CS>();
                m_IK_Arm = go.GetComponent<IK_CS>();
                if (m_IK_Arm != null)
                {
                    m_PositionWeightR = m_IK_Arm.rightHandPosWeight;
                    m_RotationWeightR = m_IK_Arm.rightHandRotWeight;
                    m_PositionWeightL = m_IK_Arm.leftHandPosWeight;
                    m_RotationWeightL = m_IK_Arm.leftHandRotWeight;
                }

			} ).AddTo(this);
	}

	private void Awake()
	{
		m_PhotonView = GetComponent<PhotonView>();
	}

	public void Play( string animation )
	{
		m_PhotonView.RPC( "OutputLog", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName, animation );

		++m_CurrentSeqNo;
		StartCoroutine( IKAdjustPlay( animation, m_CurrentSeqNo ) );
	}

	[PunRPC]
	public void OutputLog( string nickname, string animation )
	{
		// LogHelper.Log( "[" + Time.realtimeSinceStartup + "] " + nickname + " PlayAnimation -> " + animation );
	}


	IEnumerator IKAdjustPlay( string animation, int seq_no )
	{
        m_IK_Head.enabled = false;
        m_IK_Arm.rightHandPosWeight = 0f;
        m_IK_Arm.rightHandRotWeight = 0f;
        m_IK_Arm.leftHandPosWeight = 0f;
        m_IK_Arm.leftHandRotWeight = 0f;

		m_Animator.Play( animation );
		yield return null; // ステートの反映に1フレームいる
		yield return new WaitAnimation( m_Animator, 0 );

		if( m_CurrentSeqNo != seq_no )
		{
			yield break;
		}

        m_IK_Head.enabled = true;
        m_IK_Arm.rightHandPosWeight = m_PositionWeightR;
        m_IK_Arm.rightHandRotWeight = m_RotationWeightR;

        m_IK_Arm.leftHandPosWeight = m_PositionWeightL;
        m_IK_Arm.leftHandRotWeight = m_RotationWeightL;
	}
}
