using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.Events;
using VRTK;
using Photon.Pun;

public class WhiteboardTransformSwapper : MonoBehaviour
{

    private static Transform WhiteboardMoveFrom;
    private static Transform WhiteboardMoveTo;
    [SerializeField] private GameObject DrawableCanvas;
    [SerializeField] private VRTK_UICanvas AddButtonUICanvas;

    [SerializeField] private GameObject WhiteBoardMoveButton;
    private PhotonView m_PhotonView = null;

    private static Subject<GameObject> m_OnClickWhiteBoardMoveActionPanel = new Subject<GameObject>();
    public static IObservable<GameObject> OnClickWhiteBoardMoveActionPanel { get { return m_OnClickWhiteBoardMoveActionPanel; } }

    public UnityEvent OnClickWhiteBoardMoveButtonSelf = new UnityEvent();
    public UnityEvent OnClickWhiteBoardMoveButtonOther = new UnityEvent();
    public UnityEvent OnWhiteBoardMoveActionPanelClicked = new UnityEvent();
    private static bool isSelecting = false;

    // Start is called before the first frame update
    void Start()
    {
        m_PhotonView = GetComponent<PhotonView>();
        WhiteBoard.OnClickWhiteBoardMoveButton.Subscribe(go => OnClickWhiteBoardMoveButtonForAllWhiteBoard(go)).AddTo(this);
        OnClickWhiteBoardMoveActionPanel.Subscribe(_ => ClickWhiteBoardMoveActionPanelForAllWhiteBoard()).AddTo(this);

    }

    public void ClickWhiteBoardMoveActionPanel()
    {
        WhiteboardMoveTo = gameObject.transform;
        PhotonView view = gameObject.GetComponent<PhotonView>();
        if (!view.IsMine)
        {
            view.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
        }
        SwapTransform();
        m_OnClickWhiteBoardMoveActionPanel.OnNext(gameObject);

    }

    private void RefleshAddButtonUICanvas()
    {
        if (AddButtonUICanvas != null)
        {
            AddButtonUICanvas.enabled = false;
            //次のフレームで実行する
            Observable.NextFrame()
                .Subscribe(_ => AddButtonUICanvas.enabled = true).AddTo(this);
        }

    }

    //ホワイトボード移動ボタン押下時に全ホワイトボードに対して行う処理
    private void OnClickWhiteBoardMoveButtonForAllWhiteBoard(GameObject go)
    {
        isSelecting = true;
        m_PhotonView.RPC("LockWhiteBoardMoveButton", RpcTarget.Others);
        if (DrawableCanvas.Equals(go))
        {
            WhiteboardMoveFrom = gameObject.transform;
            PhotonView view = gameObject.GetComponent<PhotonView>();
            if (!view.IsMine)
            {
                view.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
            }
            OnClickWhiteBoardMoveButtonSelf.Invoke();
        }
        else
        {
            OnClickWhiteBoardMoveButtonOther.Invoke();
        }

        RefleshAddButtonUICanvas();
    }

    //
    private void ClickWhiteBoardMoveActionPanelForAllWhiteBoard()
    {
        OnWhiteBoardMoveActionPanelClicked.Invoke();
        isSelecting = false;
        //3秒後にロック解除
        Observable.Timer(TimeSpan.FromSeconds(3)).Where(_ => !isSelecting)
    .Subscribe(_ => m_PhotonView.RPC("ReleaseWhiteBoardMoveButton", RpcTarget.Others)).AddTo(this);
       
    }

    // Update is called once per frame
    void SwapTransform()
    {
        Vector3 posFrom = WhiteboardMoveFrom.position;
        Quaternion rotFrom = WhiteboardMoveFrom.rotation;
        Vector3 posTo= WhiteboardMoveTo.position;
        Quaternion rotTo = WhiteboardMoveTo.rotation;
        WhiteboardMoveTo.SetPositionAndRotation(posFrom, rotFrom);
        WhiteboardMoveFrom.SetPositionAndRotation(posTo, rotTo);



    }


    [PunRPC]
    private void LockWhiteBoardMoveButton()
    {
        WhiteBoardMoveButton.SetActive(false);
    }


    [PunRPC]
    private void ReleaseWhiteBoardMoveButton()
    {
        WhiteBoardMoveButton.SetActive(true);
    }
}
