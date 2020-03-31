using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Recollab.Network;
using Recollab.SceneManagement;
using Recollab.RoomManagement;
using UniRx.Async;
using System;
using System.IO;

using Recollab.UserManagement;

public class LobbyTabletPresenter : MonoBehaviour
{

    private AudioSource audioSource;
    [SerializeField] private GameObject PopUpPanel = null;
    [SerializeField] private Text PopUpPanel_Text_MainTitle = null;
    [SerializeField] private Text PopUpPanel_Text_Message = null;
    [SerializeField] private Button PopUpPanel_Button_Ok = null;

    [SerializeField] private GameObject RoomSelectPanel = null;
    [SerializeField] private GameObject AvatarPanel = null;
    [SerializeField] private GameObject TabController = null;
    [SerializeField] private GameObject TabPanel = null;

    // 後でCharacterSelectをPureClassに変更する
    [SerializeField] private CharacterSelect m_CharacterSelect = null;
    [SerializeField] private GameObject RoomSelectButtonHolder;

    [SerializeField] private Button Button_ActiveKeyboardInAvatarPanel;
    [SerializeField] private Button Button_EnterKey;
    [SerializeField] private GameObject WorldKeyboard;
    [SerializeField] private InputField InputField_AvatarName;
    [SerializeField] private Button Button_RemoveFromList;
    [SerializeField] private Button Button_PermanentlyRemoved;

    [SerializeField] private Button Button_BackFromAvatarPanel;
    [SerializeField] private GameObject AvatarPopUpPanel = null;
    [SerializeField] private Button Button_EnterFromAvatarPopupPanel;
    [SerializeField] private Button Button_CancelFromAvatarPopupPanel;
    [SerializeField] private Text Text_Enter;


    [SerializeField] private GameObject LibraryFolderPanel = null;
    [SerializeField] private GameObject LibraryDatePanel = null;
    [SerializeField] private Button Button_ToLibraryFolderFromDate;
    [SerializeField] private Text Text_LibraryRoomName;



    private ButtonDateLibraryPresenter SelectedLibraryButton;
    private ISceneManager m_SceneManager = null;
    private INetworkEngineConnector NetworkEngineConnector = null;
    // Oculusがnamespace使ってないのでしゃーない
    private Recollab.RoomManagement.RoomManager m_RoomManager;


    private static Subject<Unit> m_OnRoomInfoLoadComplete = new Subject<Unit>();
    public static IObservable<Unit> OnRoomInfoLoadComplete { get { return m_OnRoomInfoLoadComplete; } }

    [Inject]
    private void Initialize(
        IPlayerAvatarPresenter playerPresenter,
        ISceneManager sceneManager,
        INetworkEngineConnector networkEngineConnector)
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        m_SceneManager = sceneManager;
        NetworkEngineConnector = networkEngineConnector;
        m_RoomManager = new Recollab.RoomManagement.RoomManager();

  
        m_CharacterSelect.OnSubmitAvatarSettings
            .Subscribe(path =>
           {
            
               playerPresenter.AvatarPath = path;
               m_SceneManager.TransitionRoomScene(RoomInfo.m_CurrentRoom.RoomStageType, RoomInfo.m_CurrentRoom.RoomId);
               
               //昔入ったことあるルームなら一旦削除
               if (RoomInfo.m_Rooms.Contains(RoomInfo.m_CurrentRoom))
               {
                   RoomInfo.m_Rooms.Remove(RoomInfo.m_CurrentRoom);
               }
               RoomInfo.m_Rooms.Insert(0, RoomInfo.m_CurrentRoom);
               m_RoomManager.CreateJoinedRoomList();
           })
            .AddTo(this);


        if (UserInfo.UserId != null)
        {
            // 画面遷移
            RoomSelectPanel.SetActive(true);
        }
        

        ///ライブラリ周り
        ButtonFolderLibraryPresenter.OnClickFolderButton
            .Subscribe(x => OnClickFDRecordFolderButton(x))
            .AddTo(this);
        Button_ToLibraryFolderFromDate.onClick.AsObservable()
           .Subscribe(_ => OnClickButton_ToLibraryFolderFromDate())
           .AddTo(this);


        Button_BackFromAvatarPanel.onClick.AsObservable()
            .Subscribe(_ => OnClickButton_BackFromAvatarPanel())
            .AddTo(this);


        Button_ActiveKeyboardInAvatarPanel.onClick.AsObservable()
           .Subscribe(_ => OnClickButton_ActiveKeyboardInAvatarPanel())
           .AddTo(this);



        InputField_AvatarName.OnValueChangedAsObservable()
            .Subscribe(_ => InteractableEnterButton())
            .AddTo(this);



        ///ライブラリで一覧のボタンを選んだら
        ButtonDateLibraryPresenter.OnClickDateButton
            .Subscribe(button => ShowAlertPanel(button))
            .AddTo(this);

        RoomSelectButtonPresenter.OnClick
            .Subscribe(roomSelectButtonPresenter => OnClickRoomSelectButton(roomSelectButtonPresenter))
            .AddTo(this);

        PopUpPanel_Button_Ok.onClick.AsObservable()
            .Subscribe(_ => OnClickPopUpPanel_OkButton())
            .AddTo(this);


        ///アバター関連
        ButtonAvatarPresenter.OnClickAvatarButton
            .Subscribe(_ => OnClickAvatarButton())
            .AddTo(this);
    }


    /// <summary>
    /// ライブラリ周り
    /// </summary>


private void OnClickFDRecordFolderButton(ButtonFolderLibraryPresenter blp)
    {
        LibraryFolderPanel.SetActive(false);
        LibraryDatePanel.SetActive(true);
        Text_LibraryRoomName.text=blp.GetRoomInfo().RoomName;
    }

    private void OnClickButton_ToLibraryFolderFromDate()
    {
        LibraryFolderPanel.SetActive(true);
        LibraryDatePanel.SetActive(false);
    }


    /// <summary>
    /// Avatar関連メソッド
    /// </summary>

    private void OnClickAvatarButton()
    {
        AvatarPopUpPanel.SetActive(true);
        WorldKeyboard.SetActive(true);
        InputField_AvatarName.text = UserInfo.UserId;
        WorldKeyboard.GetComponent<UI_Keyboard>().SetInputField(InputField_AvatarName);
    }
    private void OnClickButton_AvatarCancel()
    {
        AvatarPopUpPanel.SetActive(false);
        WorldKeyboard.SetActive(false);
    }


    /// <summary>
    /// 画面遷移ボタンメソッド
    /// </summary>
    private void OnClickButton_CancelFromNewRoomCreatePanel()
    {
        WorldKeyboard.SetActive(false);
        RoomSelectPanel.SetActive(true);
        
    }
    private void OnClickButton_CancelFromNewRoomTypeSelectPanel()
    {
        WorldKeyboard.SetActive(false);

    }

    private void OnClickButton_BackFromAvatarPanel()
    {
        AvatarPanel.SetActive(false);
        RoomSelectPanel.SetActive(true);
    }

    private void OnClickButton_Cancel()
    {
        WorldKeyboard.SetActive(false);
        RoomSelectPanel.SetActive(true);
    }


    private void OnClickButton_ActiveKeyboardInSelectPanel()
    {
        WorldKeyboard.SetActive(true);
           RoomSelectPanel.SetActive(false);

    }
    private void OnClickButton_ActiveKeyboardInSearchPanel()
    {
        WorldKeyboard.SetActive(true);

    }

    private void OnClickButton_ActiveKeyboardInNewRoomCreatePanel()
    {
        WorldKeyboard.SetActive(true);
    }

    private void OnClickButton_ActiveKeyboardInAvatarPanel()
    {
        WorldKeyboard.SetActive(true);
        WorldKeyboard.GetComponent<UI_Keyboard>().SetInputField(InputField_AvatarName);
    }

    private void OnClickButton_ActiveKeyboardInUserID()
    {
        WorldKeyboard.SetActive(true);
    }

    private void OnClickButton_ActiveKeyboardInPW()
    {
        WorldKeyboard.SetActive(true);
    }

    private void InteractableEnterButton()
    {
        if (string.IsNullOrEmpty(InputField_AvatarName.text)) {
            Button_EnterFromAvatarPopupPanel.interactable = false;
            Text_Enter.color = new Color(142 / 256f, 142 / 256f, 147 / 256f, 1.0f);
        }
        else
        {
            Button_EnterFromAvatarPopupPanel.interactable = true;
            Text_Enter.color = new Color(0 / 256f, 122 / 256f, 255 / 256f, 1.0f);
        }
    }

    /// <summary>
    /// UIオンオフ
    /// </summary>
    private void OnClickPopUpPanel_OkButton()
    {
        audioSource.Play();
        NetworkEngineConnector.OnDisconnectedAsObservable.First().Subscribe(__ => {
            NetworkEngineConnector.SetOfflineMode();
            //First()でDisposeされるのでAddto(this)しないこと（シーン遷移でオブジェクトが破壊されるため）
            m_SceneManager.OnRoomLoaded.First().DelayFrame(5).Subscribe(_ => OnPlay());

            m_CharacterSelect.Submit4FDRecord();

        });
        NetworkEngineConnector.Disconnect();
    }

    private void OnClickRoomSelectButton(RoomSelectButtonPresenter roomSelectButtonPresenter)
    {
        SelectRoomAndShowAvatarPanel(roomSelectButtonPresenter.GetRoomInfo());
    }

    private void ShowAlertPanel(ButtonDateLibraryPresenter button)////ルーム選択、アバター表示
    {
        PopUpPanel_Button_Ok.interactable = false;
        PopUpPanel_Text_MainTitle.text = button.GetDateText();
        PopUpPanel_Text_Message.text = "ファイルの読み込みに 時間のかかる場合があります";
        PopUpPanel.SetActive(true);
        SelectedLibraryButton = button;
        Debug.Log("ターゲットデータパス" + SystemDefines.FDRECORD_PATH + SelectedLibraryButton.targetDataPath);

    }

    private void SelectRoomAndShowAvatarPanel(Room room)
    {
        RoomSelectPanel.SetActive(false);
        AvatarPanel.SetActive(true);
        audioSource.Play();
        RoomInfo.m_CurrentRoom = room;

    }

    private void OnPlay()///OKボタンを押したとき
    {
        if (Directory.Exists(SystemDefines.FDRECORD_PATH + SelectedLibraryButton.targetDataPath))
        {
            // ToDo : 4DRecordLibrary.Play(SelectedLibraryButton.targetDataPath);
        }
    }
}
