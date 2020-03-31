using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CreateSticky : MonoBehaviourPunCallbacks
{

    private static Subject<Unit> m_OnCreateStickyRPC = new Subject<Unit>();
    public static IObservable<Unit> OnCreateStickyRPC { get { return m_OnCreateStickyRPC; } }
    private PhotonView m_PhotonView = null;
    private StickeyColor StickyType = StickeyColor.Yellow;
    [SerializeField] private GameObject stickyHolder;

    // Use this for initialization
    void Start()
    {
        m_PhotonView = GetComponent<PhotonView>();
        HandWritingStickeyController.OnHandWritingStickeyComplete.Subscribe(x => CreateSyncStickywithRawImage(x)).AddTo(this);

    }

    public void ChangeStickyType(StickeyColor type)
    {
        StickyType = type;
    }

    public GameObject CreateSyncTransparentSticky()
    {
        return PhotonNetwork.Instantiate("TransparentSticky", Camera.main.transform.TransformPoint(new Vector3(0, 0, 1)), Quaternion.identity, 0);

    }


    public GameObject CreateSyncNormalSticky()
    {
        return PhotonNetwork.Instantiate("Sticky", Camera.main.transform.TransformPoint(new Vector3(0, 0, 1)), Quaternion.identity, 0);

    }


    public GameObject CreateSyncOrangeSticky()
    {
        return PhotonNetwork.Instantiate("StickyOrange", Camera.main.transform.TransformPoint(new Vector3(0, 0, 1)), Quaternion.identity, 0);

    }


    public GameObject CreateSyncGreenSticky()
    {
        return PhotonNetwork.Instantiate("StickyGreen", Camera.main.transform.TransformPoint(new Vector3(0, 0, 1)), Quaternion.identity, 0);

    }


    public GameObject CreateSyncBlueSticky()
    {
        return PhotonNetwork.Instantiate("StickyBlue", Camera.main.transform.TransformPoint(new Vector3(0, 0, 1)), Quaternion.identity, 0);

    }


    public GameObject CreateSyncYellowSticky()
    {
        return PhotonNetwork.Instantiate("StickyYellow", Camera.main.transform.TransformPoint(new Vector3(0, 0, 1)), Quaternion.identity, 0);

    }

    private void SetupSticky(GameObject stikcy, string text)
    {
        stikcy.transform.parent = stickyHolder.transform;
        stikcy.transform.localPosition = Vector3.zero;
        stikcy.transform.localRotation = Quaternion.identity;

        Sticky sk = stikcy.GetComponent<Sticky>();
        sk.SetText(text);
    }

    private void SetupSticky(GameObject stikcy, Texture2D tex2D)
    {
        stikcy.transform.parent = stickyHolder.transform;
        stikcy.transform.localPosition = Vector3.zero;
        stikcy.transform.localRotation = Quaternion.identity;

        Sticky sk = stikcy.GetComponent<Sticky>();
        sk.SetTexture(tex2D);
    }

    public void SetStickeyColorTransparent()
    {
        StickyType = StickeyColor.Transparent;
    }



    private GameObject CreateSelectedStickey()
    {
        switch (StickyType)
        {
            case StickeyColor.Orange:

                return CreateSyncOrangeSticky();

            case StickeyColor.Green:
                return CreateSyncGreenSticky();

            case StickeyColor.Blue:
                return CreateSyncBlueSticky();

            case StickeyColor.Yellow:
                return CreateSyncYellowSticky();

            case StickeyColor.Transparent:
                return CreateSyncTransparentSticky();

            default:
                Debug.LogError("想定されていないタイプが入力されました : " + StickyType);
                return CreateSyncNormalSticky();
        }
    }

    public void CreateSyncStickywithRawImage(Texture2D tex2D)
    {
        GameObject sticky = CreateSelectedStickey();
        SetupSticky(sticky, tex2D);

        m_PhotonView.RPC("CreateStickyRPC", RpcTarget.All);
    }

    // ToDo : CreateSyncStickyとまとめるか要検討
    public void CreateSyncStickywithText(string text)
    {
        GameObject sticky = CreateSelectedStickey();
        SetupSticky(sticky,text);

        m_PhotonView.RPC("CreateStickyRPC", RpcTarget.All);

    }

    public void CreateSyncStickywithText(Text text)
    {

        CreateSyncStickywithText(text.text);
    }

    [PunRPC]
    private void CreateStickyRPC()
    {
        m_OnCreateStickyRPC.OnNext(Unit.Default);
    }


}
public enum StickeyColor
{
    Orange,
    Green,
    Blue,
    Yellow,
    Transparent

};
