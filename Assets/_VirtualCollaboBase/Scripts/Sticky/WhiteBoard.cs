using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VRTK;
using System;
using UnityEngine.Events;
using Zenject;
using Recollab.SceneManagement;

public class WhiteBoard : MonoBehaviour
{
    private static Subject<GameObject> m_OnClickWhiteBoard = new Subject<GameObject>();
    public static IObservable<GameObject> OnClickWhiteBoard { get { return m_OnClickWhiteBoard; } }

    private static Subject<GameObject> m_OnClickWhiteBoardMoveButton = new Subject<GameObject>();
    public static IObservable<GameObject> OnClickWhiteBoardMoveButton { get { return m_OnClickWhiteBoardMoveButton; } }
    public static GameObject DefaultTargetWhiteboard;

    [SerializeField] private bool isDefaultTargetWhiteboard = false;

    [SerializeField] private VRTK_UICanvas uICanvas = null;
    [SerializeField] private Camera WhiteboardUICamera = null;


    void Awake()
    {
        if (isDefaultTargetWhiteboard)
        {
            DefaultTargetWhiteboard = gameObject;
            // 初期ホワイトボード以外はタブレット用のカメラオン
            WhiteboardUICamera.gameObject.SetActive(true);
        }

    }

	[Inject]
    private void Initialize( ISceneManager scene_manager )
    {
		CreateSticky.OnCreateStickyRPC
			.Subscribe( _ => RefleshWhiteBoard() )
			.AddTo( this );

		scene_manager.OnRoomLoaded
			.Subscribe( _ => RefleshWhiteBoard() )
			.AddTo( this );

		TabletDisplaySwitchPresenter.OnTabletActive
			.Subscribe( _ => RefleshWhiteBoard() )
			.AddTo( this );

		OnClickWhiteBoardMoveButton
			.Subscribe( _ => RefleshWhiteBoard() )
			.AddTo( this );
    }

    public void SetWhiteboardUICameraOff()
    {
        WhiteboardUICamera.gameObject.SetActive(false);
    }

    public RenderTexture GetWhiteboardUICameraTexture()
    {
        WhiteboardUICamera.gameObject.SetActive(true);
        return WhiteboardUICamera.targetTexture;
    }

    private void RefleshWhiteBoard()
    {
        if (uICanvas != null)
        {
            uICanvas.enabled = false;
            //次のフレームで実行する
            Observable.NextFrame()
                .Subscribe(_ => uICanvas.enabled = true).AddTo(this);
        }

    }

    public void ClickWhiteBoardMoveButton()
    {
        m_OnClickWhiteBoardMoveButton.OnNext(gameObject);

    }



    public void ClickWhiteBoard()
    {

        m_OnClickWhiteBoard.OnNext(gameObject);
        Debug.Log("###########     OnClickWhiteBoard    ###########");

    }
}
