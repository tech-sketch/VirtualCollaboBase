using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class ButtonAvatarPresenter : MonoBehaviour
{
    [SerializeField] private Text Text_AvatarName;
    [SerializeField] private Image Image_AvatarThumbnail;
    private GameObject Avatar;
    private static Subject<GameObject> m_OnClickAvatarButton = new Subject<GameObject>();　///自分自身を送る
    public static IObservable<GameObject> OnClickAvatarButton { get { return m_OnClickAvatarButton; } }


    public void OnClicked()
    {
        m_OnClickAvatarButton.OnNext(Avatar);
    }

    public void SetAvatarInfo(GameObject avatar)
    {
        VRMMeta vRMMeta = avatar.GetComponentInChildren<VRMMeta>();
        if (vRMMeta != null)
        {
            Text_AvatarName.text = vRMMeta.Meta.Title;
            Image_AvatarThumbnail.sprite = Sprite.Create(vRMMeta.Meta.Thumbnail, new Rect(0, 0, vRMMeta.Meta.Thumbnail.width, vRMMeta.Meta.Thumbnail.height), Vector2.zero);
            Avatar = avatar;
        }
        else
        {
            Debug.LogError(avatar.name + "はVRMアバターではありません。");
        }

       
    }


}
