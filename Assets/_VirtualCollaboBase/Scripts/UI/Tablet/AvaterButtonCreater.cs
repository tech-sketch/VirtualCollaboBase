using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AvaterButtonCreater : MonoBehaviour
{

    [SerializeField] private GameObject ButtonAvatarPrefab;
    [SerializeField] private GameObject AvatarHolder;


    [Inject]
    private void Initialize(IAvatarSettingPresenter avatarSettingPresenter)
    {

        foreach (var avatar in avatarSettingPresenter.UseAvatarList)
        {
            CreateAvatarButton(avatar);
        }

    }

    private void CreateAvatarButton(GameObject avatar)
    {
        GameObject button = Instantiate(ButtonAvatarPrefab);
        ButtonAvatarPresenter bap = button.GetComponent<ButtonAvatarPresenter>();

        bap.SetAvatarInfo(avatar);

        button.transform.parent = AvatarHolder.transform;
        button.transform.localPosition = Vector3.zero;
        button.transform.localRotation = Quaternion.identity;
    }




}
