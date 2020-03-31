using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UniRx;
using GuidBasedReference;
using SimpleDrawing;
using System.IO;
using Zenject;
using Recollab.SceneManagement;

public class DrawableWhiteBoardPresenter : MonoBehaviour
{

    [SerializeField] private GameObject WhiteBoard = null;
    [SerializeField] private GameObject AddButtonRoot = null;
    [SerializeField] private VRTK_UICanvas AddButtonUICanvas = null;
    private PhotonView m_PhotonView = null;


	[Inject]
    private void Initialize( ISceneManager scene_manager )
    {
        m_PhotonView = GetComponent<PhotonView>();

		scene_manager.OnRoomLoaded
			.Subscribe( _ => RefleshAddButtonUICanvas() )
			.AddTo( this );

		TabletDisplaySwitchPresenter.OnTabletActive
			.Subscribe( _ => RefleshAddButtonUICanvas() )
			.AddTo( this );

		CreateSticky.OnCreateStickyRPC
			.Subscribe( _ => RefleshAddButtonUICanvas() )
			.AddTo( this );
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

    public void SetWhiteBoardActive()
    {
        m_PhotonView.RPC("SetWhiteBoardActiveRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void SetWhiteBoardActiveRPC()
    {
        if (AddButtonRoot != null)
        {
            AddButtonRoot.SetActive(false);
            WhiteBoard.SetActive(true);
            TextureSharing4Whiteboard sharing4Whiteboard = gameObject.GetComponentInChildren<TextureSharing4Whiteboard>();
            Observable.TimerFrame(1).Subscribe(_ => sharing4Whiteboard.StartTextureSharing()).AddTo(this);
        }
    }
}
