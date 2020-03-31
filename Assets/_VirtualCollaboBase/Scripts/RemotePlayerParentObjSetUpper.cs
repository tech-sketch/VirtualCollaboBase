using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ToDo : 暫定実装のクラスなので将来的に削除予定
public class RemotePlayerParentObjSetUpper : MonoBehaviour
{
    [SerializeField] private PhotonView m_PhotonView = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_PhotonView.IsMine && PhotonNetwork.InRoom)
        {
            gameObject.transform.parent = SharingRoot.m_sharingRoot.transform;
        }
    }
}
