using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Photon.Pun;
using System;
using UnityEngine.UI;
using VRM;
using MecanimIKPlus;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private Button Button_EnterFromAvatarPopupPanel;
    [SerializeField] private Button Button_CancelFromAvatarPopupPanel;
    [SerializeField] private Text Text_InputFieldAvatarName;
    [SerializeField] private Transform AvatarPreviewPos;
    private GameObject PreviewAvatarObj;
    //[SerializeField] private Spawner m_Spawner = null;

	private Subject<string> m_SubmitAvatarSettingsSubject = new Subject<string>();
	public IObservable<string> OnSubmitAvatarSettings { get { return m_SubmitAvatarSettingsSubject; } }
    private GameObject SelectedAvatar;

    private void Start()
    {
        Button_EnterFromAvatarPopupPanel.onClick.AsObservable()
           .Subscribe(_ => OnClickenterButton())
           .AddTo(this);

        Button_CancelFromAvatarPopupPanel.onClick.AsObservable()
   .Subscribe(_ => OnClickCancelButton())
   .AddTo(this);

        ButtonAvatarPresenter.OnClickAvatarButton.Subscribe(avatar => SelectAvatar(avatar));
    }

    private void OnClickCancelButton()
    {
        if (PreviewAvatarObj != null)
        {
            Destroy(PreviewAvatarObj);
            PreviewAvatarObj = null;
        }
    }

    private void OnClickenterButton()
    {
        SetNickName(Text_InputFieldAvatarName.text);
        Submit();
    }

    private void SelectAvatar(GameObject Avatar)
    {
        PreviewAvatar(Avatar);

    }

    public void PreviewAvatar( GameObject Avatar)
	{
        if (PreviewAvatarObj != null)
        {
            Destroy(PreviewAvatarObj);
            PreviewAvatarObj = null;
        }

        SelectedAvatar = Avatar;
        PreviewAvatarObj = Instantiate(Avatar.GetComponentInChildren<VRMMeta>().gameObject);
        PreviewAvatarObj.transform.parent = AvatarPreviewPos;
        PreviewAvatarObj.transform.localPosition = Vector3.zero;
        PreviewAvatarObj.transform.localScale = new Vector3(100f, 100f, 100f);
        PreviewAvatarObj.transform.localRotation = Quaternion.Euler(new Vector3(0f,180f,0f));
        foreach (var springBone in PreviewAvatarObj.GetComponentsInChildren<VRMSpringBone>())
        {
            springBone.enabled = false;
        }

        var ik_Head = PreviewAvatarObj.GetComponent<IK_Head_Linkage_CS>();
        var ik_Arm = PreviewAvatarObj.GetComponent<IK_CS>();


        if (ik_Head != null)
        {
            ik_Head.enabled = false;
            ik_Arm.enabled = false;
        }
        

    }

	public void SetNickName( string name )
	{
		PhotonNetwork.LocalPlayer.NickName = name;
	}

	public void Submit()
	{
		string avatar_path = SystemDefines.PLAYABLE_AVATAR_PATH + SelectedAvatar.name;
		m_SubmitAvatarSettingsSubject.OnNext( avatar_path );
		//m_Spawner.Spawn( avatar_path );
	}

    public void Submit4FDRecord()
    {
        string avatar_path = SystemDefines.PLAYABLE_AVATAR_PATH + SelectedAvatar.name;
        m_SubmitAvatarSettingsSubject.OnNext(avatar_path);
        //m_Spawner.Spawn4FDRecord(avatar_path);
    }
}
