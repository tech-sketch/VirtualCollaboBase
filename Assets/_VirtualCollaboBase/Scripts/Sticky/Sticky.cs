using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using Photon.Pun;
using GuidBasedReference;

public class Sticky : MonoBehaviour {

    private static Subject<GameObject> m_OnClickSticky = new Subject<GameObject>();
    public static IObservable<GameObject> OnClickSticky { get { return m_OnClickSticky; } }


    public Text displayText;
    public RawImage displayImage;
    private PhotonView m_PhotonView = null;


    // Use this for initialization
    void Start () {

        m_PhotonView = GetComponent<PhotonView>();
    }
	
	// Update is called once per frame
	public void SetText (string text) {
        displayText.text = text;
    }

    public void SetTexture(Texture2D tex2D)
    {
        displayImage.texture = tex2D;
        displayImage.color = new Color(1, 1, 1, 1);
        TextureSharing4Sticky textureSharing4 = gameObject.GetComponent<TextureSharing4Sticky>();
        textureSharing4.StartTextureBroadcast(tex2D);

        //マスターは後から来た人用のテクスチャの共有の設定を行う。
        if (PhotonNetwork.IsMasterClient)
        {
            textureSharing4.SetTexture4TextureSharing(tex2D);
        }
        

    }


    public void SetParent(GameObject targetWhiteboard)
    {
        GuidComponent guidComponent = targetWhiteboard.GetComponent<GuidComponent>();
        m_PhotonView.RPC("SetParentRPC", RpcTarget.AllBuffered, guidComponent.GetGuid().ToString());

    }

    [PunRPC]
    private void SetParentRPC(string guid)
    {
        GameObject whiteBoard = null;
        GuidComponentManager.Instance.GuidToSceneObjectMap.TryGetValue(guid, out whiteBoard);
        gameObject.transform.parent = whiteBoard.transform;

    }

    public void RemoveParent()
    {
        m_PhotonView.RPC("RemoveParentRPC", RpcTarget.OthersBuffered);

    }

    [PunRPC]
    private void RemoveParentRPC()
    {
        gameObject.transform.parent = null;

    }

    public void ClickSticky()
    {

        m_OnClickSticky.OnNext(gameObject);

    }
}
