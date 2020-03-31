using UnityEngine;
using System.IO;
using VRM;
using MecanimIKPlus;
using UniRx;
using VRTK;
using Photon.Pun;
using System;

public interface IRuntimeModelLoader
{
	void InstantiateModel( string path );

	IObservable<GameObject> OnLoadModelCompleted { get; }
	IObservable<GameObject> OnMiniAvatarLoadCompleted { get; }
}

public class VRMRuntimeLoader : MonoBehaviour, IRuntimeModelLoader
{
	[Header("MainAvatar Settings")]
	[SerializeField] private string m_Layer;
	[SerializeField] private RuntimeAnimatorController m_AnimatorController;
	[SerializeField] private GameObject VRMRoot;
    [SerializeField] private GameObject m_LeftHandTarget;
	[SerializeField] private GameObject m_RightHandTarget;

	public IObservable<GameObject> OnLoadModelCompleted { get { return m_LoadModelCompletedSubject; } }
	private Subject<GameObject> m_LoadModelCompletedSubject = new Subject<GameObject>();
	public IObservable<GameObject> OnMiniAvatarLoadCompleted { get { return m_MiniAvatarLoadCompleted; } }
	private Subject<GameObject> m_MiniAvatarLoadCompleted = new Subject<GameObject>();

	private ResourceInstantiater m_Instantiater = new ResourceInstantiater();

	void Start()
	{
		m_Instantiater.OnInstantiateResourceCompleted
			.Subscribe( go =>
			{
				OnLoadedVrm( go.m_Model );
			} )
			.AddTo( this );
	}

	public void InstantiateModel( string path )
	{
		StartCoroutine(
			m_Instantiater.InstantiateResourceAsyncNetwork( path, transform.position, Quaternion.identity, transform, 0 ) );
	}

	private void OnLoadedVrm( GameObject avatarObject )
	{
		LoadMainAvatar(avatarObject);
	}

	private void LoadMainAvatar( GameObject avatarObject )
	{
        avatarObject.transform.SetParent( VRMRoot.transform, false );
        avatarObject.SetLayerRecursively( LayerMask.NameToLayer( m_Layer ) );
        GameObject vrm = avatarObject.GetComponentInChildren<VRMMeta>().gameObject;

        Animator animator = vrm.gameObject.GetComponent<Animator>();
		animator.runtimeAnimatorController = m_AnimatorController;

		// VRIKのセットアップ
		SetupVRIK( vrm, VRTK_DeviceFinder.HeadsetTransform().gameObject, m_LeftHandTarget, m_RightHandTarget );
		SetupAvatarTransfer(avatarObject);

		m_LoadModelCompletedSubject.OnNext( vrm );
	}

	private void SetupVRIK( GameObject avatar, GameObject headTarget, GameObject leftHandTarget, GameObject rightHandTarget )
	{

		// VRIKを設定
		var IK_Head = avatar.GetComponentInChildren<IK_Head_Linkage_CS>();
        var IK_CS = avatar.GetComponentInChildren<IK_CS>();
;

        //HiddenにはIKないのでチェック
        if (IK_Head != null)
        {
            // 頭や腕のターゲット設定
            IK_Head.eyeTransform = headTarget.transform;
            IK_CS.lookAtTarget = headTarget.transform.GetChild(0);
            //Bodyのtransformを探す
            IK_CS.bodyTarget = headTarget.transform.parent.gameObject.GetComponentInChildren<IK_Body_Linkage_CS>().gameObject.transform;
            IK_CS.leftHandTarget = leftHandTarget.transform;
            IK_CS.rightHandTarget = rightHandTarget.transform;
        }


        // カメラに顔面映り込み防止のため、ニアクリップを変更（これはアバターによる）
        Camera.main.nearClipPlane = 0.25f;
	}

	private void SetupAvatarTransfer(GameObject avatar)
	{
		VRIKSolverTransfer transferSource = avatar.GetComponentInChildren<VRIKSolverTransfer>();

        //HiddenにはIKないのでチェック
        if (transferSource != null)
        {
            transferSource.IkSolverRef = IKSoverReference.AsSource;

            var IK_Head = avatar.GetComponentInChildren<IK_Head_Linkage_CS>();
            var IK_Arm = avatar.GetComponentInChildren<IK_CS>();
            if (IK_Head != null)
            {
                transferSource.HeadTarget = IK_Head.eyeTransform;
                transferSource.LeftArmTarget = IK_Arm.leftHandTarget;
                transferSource.RightArmTarget = IK_Arm.rightHandTarget;
            }

        }

	}
}