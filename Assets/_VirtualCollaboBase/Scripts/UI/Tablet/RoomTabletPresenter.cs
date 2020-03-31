using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using VRWS.SimpleDrawing.Sharing;
using Zenject;
using SimpleDrawing;
using UniRx.Triggers;
using SimpleDrawing.Sharing;
using Recollab.SceneManagement;
using Photon;
using Photon.Pun;

public class RoomTabletPresenter : MonoBehaviour
{

    [SerializeField] AudioSource audioSource;

    [SerializeField] Slider m_WhiteboardScaleSlider;
    [SerializeField] GameObject m_WBDrawableCanvas;
    [SerializeField] Button Button_ScaleMinus;
    [SerializeField] Button Button_ScalePlus;
    [SerializeField]
    private ObservableDragTrigger CanvasDragTrigger;
    [SerializeField] LocalSyncBoard m_LocalSyncBoard;
    [SerializeField] private Button m_SaveWhiteboardButton;

    [SerializeField] private GameObject StickeyColorHeader;
    [SerializeField] private Button m_button_husenYellow;
    [SerializeField] private Button m_button_husenOrange;
    [SerializeField] private Button m_button_husenGreen;
    [SerializeField] private Button m_button_husenBlue;
    [SerializeField] private Button m_button_textExport;
    [SerializeField] private Material StickyYellow;
    [SerializeField] private Material StickyOrange;
    [SerializeField] private Material StickyGreen;
    [SerializeField] private Material StickyBlue;
    [SerializeField] private Material StickyTransparent;
    [SerializeField] private Button Button_StickeyCreate;
    [SerializeField] private Button Button_StickeyCreateCancel;

    [SerializeField] private DrawableCanvas StickeyCanvas;
    [SerializeField] private GameObject PenInputHeader;

    [SerializeField] private GameObject MicInputHeader;
    [SerializeField] private Button m_MicStart_Button;
    [SerializeField] private Button m_MicEnd_Button;
    [SerializeField] private Button m_MicFail_Button;
    [SerializeField] private Text text_Result;
    [SerializeField] private Text text_RecordState;
    [SerializeField] private Text text_RecordComment;
    [SerializeField] private GameObject image_MicProgress;
    [SerializeField] GameObject m_CompletionMessageObject;
    [SerializeField] float m_CompletionMessageTimeSeconds;
    [SerializeField] private Image TextInputPannelBG;

    [SerializeField] private CreateSticky m_CreateSticky;

    [SerializeField] private GameObject ExitPopupPanel;
    [SerializeField] private SceneController SceneController;
    [SerializeField] private Button Button_Exit;
    [SerializeField] private Button Button_ExitfromRoomAfterSave;
    [SerializeField] private Button Button_ExitfromRoomWithoutSave;


    [SerializeField] private GameObject TabContorller;
    [SerializeField] private GameObject BroserInputPanel;
    [SerializeField] private GameObject ObjectAnchor;

    string m_LocalDirPath = Application.streamingAssetsPath + "/WhiteboardExporter";

    Transform m_WBDrawableCanvasTransform;
    Vector3 m_WBDrawableCanvasScale;
    Renderer m_WBDrawableCanvasRenderer;
    public static float m_TilingValue = 1;
    public static float m_OffsetgValueX = 0;
    public static float m_OffsetgValueY = 0;

    private readonly static float DRAG_DISTANCE_VALUE = -250.0f;
    private readonly static float MAX_TILING_VALUE = 1.0f;
    private readonly static float MIN_TILING_VALUE = 0.0f;
    private readonly static float MIN_OFFSET_VALUE = 0.0f;
    private readonly static float MAX_RANGE = 1.0f;
    private readonly static float ADD_TILING_VALUE = 0.1f;

    private static Subject<Unit> m_OnRoomSave = new Subject<Unit>();
    public static IObservable<Unit> OnRoomSave { get { return m_OnRoomSave; } }

    private static Subject<Texture2D> m_OnShareScreen = new Subject<Texture2D>();
    public static IObservable<Texture2D> OnShareScreen { get { return m_OnShareScreen; } }


    private void Start()
    {
        m_SaveWhiteboardButton.onClick.AsObservable().Subscribe(_ => OnClickeSaveWhiteboardButton()).AddTo(this);

        m_button_husenYellow.onClick.AsObservable().Subscribe(_ => ChangeStickyType(StickyYellow, StickeyColor.Yellow)).AddTo(this);
        m_button_husenOrange.onClick.AsObservable().Subscribe(_ => ChangeStickyType(StickyOrange, StickeyColor.Orange)).AddTo(this);
        m_button_husenGreen.onClick.AsObservable().Subscribe(_ => ChangeStickyType(StickyGreen, StickeyColor.Green)).AddTo(this);
        m_button_husenBlue.onClick.AsObservable().Subscribe(_ => ChangeStickyType(StickyBlue, StickeyColor.Blue)).AddTo(this);
        m_button_textExport.onClick.AsObservable().Subscribe(_ => ChangeStickyType(StickyTransparent, StickeyColor.Transparent)).AddTo(this);

        m_WBDrawableCanvasTransform = m_WBDrawableCanvas.transform;
        m_WBDrawableCanvasScale = m_WBDrawableCanvasTransform.localScale;
        m_WBDrawableCanvasRenderer = m_WBDrawableCanvas.GetComponent<Renderer>();

        Button_ScalePlus.onClick.AsObservable().Subscribe(_ => ScalePlusButton()).AddTo(this);
        Button_ScaleMinus.onClick.AsObservable().Subscribe(_ => ScaleMinusButton()).AddTo(this);

        CanvasDragTrigger.OnDragAsObservable().Subscribe(x => SetDragCanvasVector(x.delta)).AddTo(this);
        RemoteRayCastDrawer.OnDrawStart.Subscribe(_ => PenHeaderActiveChange()).AddTo(this);

        Button_Exit.onClick.AsObservable().Subscribe(_ => OnClickButton_Exit()).AddTo(this);
        Button_ExitfromRoomAfterSave.onClick.AsObservable().Subscribe(_ => m_OnRoomSave.OnNext(Unit.Default)).AddTo(this);
        Button_ExitfromRoomWithoutSave.onClick.AsObservable().Subscribe(_ => ExitFromRoom()).AddTo(this);
    }

    private void OnClickButton_BrowserPanel()
    {
        TabContorller.SetActive(false);
        BroserInputPanel.SetActive(true);
        ObjectAnchor.SetActive(false);

    }


    private void OnClickButton_CloseBrowserPanel()
    {
        BroserInputPanel.SetActive(false);
        TabContorller.SetActive(true);
        ObjectAnchor.SetActive(true);

    }

    private void ChangeStickyType(Material stickyColor, StickeyColor color)
    {
        audioSource.Play();
        m_CreateSticky.ChangeStickyType(color);
        ChangeColor(stickyColor);

        if (color == StickeyColor.Orange)
        {
            OnTextWhite();
        }
        else
        {
            OnTextRed();
        }

    }

    public void ChangeColor(Material stickyColor)
    {
        TextInputPannelBG.material = stickyColor;
        StickeyCanvas.ResetColor = stickyColor.color;
        StickeyCanvas.ResetCanvas();
    }

    ///ルーム退出と空間保存

    private void OnClickButton_Exit()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitPopupPanel.SetActive(true);

        }
        else
        {
            ExitFromRoom();
        }
    }


    private void ExitFromRoom()
    {
        ExitPopupPanel.SetActive(false);
        SceneController.TransitionLobbyScene();
    }

    private void OnClickeSaveWhiteboardButton()
    {
        Debug.Log("*** OnClickeSaveWhiteboardButton ***");
        string dateTimeStr = DateTime.Now.ToString("yyyyMMddHHmmss");
        string filename = "WhiteBoard_" + dateTimeStr + ".png";
        Texture2D tex2d = m_LocalSyncBoard.GetCurrentWhiteboradAsTexture2D();


    }


    public void OnTextWhite()
    {
        text_RecordState.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        text_RecordComment.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void OnTextRed()
    {
        text_RecordState.color = new Color(223 / 256f, 44 / 256f, 44 / 256f, 1.0f);
        text_RecordComment.color = new Color(223 / 256f, 44 / 256f, 44 / 256f, 1.0f);
    }


    private void PenHeaderActiveChange()
    {
        StickeyColorHeader.SetActive(false);
        PenInputHeader.SetActive(true);
    }

    /// <summary>
    /// 以下ホワイトボードの拡縮まわり
    /// </summary>
    public void GetScaleSliderValue()
    {
        float scaleSliderValue = m_WhiteboardScaleSlider.value;
        ChangeScaleValue(scaleSliderValue);

    }

    private void ScalePlusButton()
    {
        m_WhiteboardScaleSlider.value += ADD_TILING_VALUE;

    }

    private void ScaleMinusButton()
    {
        m_WhiteboardScaleSlider.value -= ADD_TILING_VALUE;

    }

    private void ChangeScaleValue(float scaleSliderValue)
    {

        Vector2 nowOffset = m_WBDrawableCanvasRenderer.material.mainTextureOffset;
        Vector2 nowTiling = m_WBDrawableCanvasRenderer.material.mainTextureScale;
        m_TilingValue = MAX_TILING_VALUE - scaleSliderValue;

        if (m_OffsetgValueX >= MIN_OFFSET_VALUE)
        {
            if ((m_TilingValue + nowOffset.x) >= MAX_RANGE)
            {
                m_OffsetgValueX = MAX_TILING_VALUE - m_TilingValue;
            }
            else if (nowOffset.x + (nowTiling.x - m_TilingValue) / 2 <= MIN_OFFSET_VALUE) ///縮小してOffsetが０以下になるとき
            {
                m_OffsetgValueX = MIN_OFFSET_VALUE;
            }
            else if (m_TilingValue - nowTiling.x > MIN_TILING_VALUE)///縮小したとき
            {
                m_OffsetgValueX = nowOffset.x + (nowTiling.x - m_TilingValue) / 2;
            }

            else ///拡大したとき
            {
                m_OffsetgValueX = nowOffset.x + (nowTiling.x - m_TilingValue) / 2;

            }
        }
        if (m_OffsetgValueY >= MIN_TILING_VALUE)
        {
            if ((m_TilingValue + nowOffset.y) >= MAX_RANGE)
            {
                m_OffsetgValueY = MAX_TILING_VALUE - m_TilingValue;
            }

            else if (nowOffset.y + (nowTiling.y - m_TilingValue) / 2 <= MIN_OFFSET_VALUE)///縮小してOffsetが０以下になるとき
            {
                m_OffsetgValueY = MIN_OFFSET_VALUE;
            }
            else if (m_TilingValue - nowTiling.y > MIN_TILING_VALUE)///縮小したとき
            {
                m_OffsetgValueY = nowOffset.y + (nowTiling.y - m_TilingValue) / 2;

            }
            else ///拡大したとき
            {
                m_OffsetgValueY = nowOffset.y + (nowTiling.y - m_TilingValue) / 2;
            }
        }
        ChangeWhiteboardTextureUVTiling(m_TilingValue);
        ChangeWhiteboardTextureUVOffset(m_OffsetgValueX, m_OffsetgValueY);
    }

    private void SetDragCanvasVector(Vector2 deltaValue)
    {

        float dragValueX = deltaValue.x / DRAG_DISTANCE_VALUE;
        float dragValueY = deltaValue.y / DRAG_DISTANCE_VALUE;
        Vector2 nowOffset = m_WBDrawableCanvasRenderer.material.mainTextureOffset;
        Vector2 nowTiling = m_WBDrawableCanvasRenderer.material.mainTextureScale;

        if (0 <= nowOffset.x + dragValueX && 0 <= nowOffset.y + dragValueY && nowTiling.x + nowOffset.x + dragValueX < 1 && nowTiling.y + nowOffset.y + dragValueY < 1)
        {
            ChangeWhiteboardTextureUVOffset(nowOffset.x + dragValueX, nowOffset.y + dragValueY);
        }
        m_OffsetgValueX = nowOffset.x + dragValueX;
        m_OffsetgValueY = nowOffset.y + dragValueY;
    }

    private void ChangeWhiteboardTextureUVTiling(float m_TilingValue)
    {

        Vector2 tiling = new Vector2(m_TilingValue, m_TilingValue);
        m_WBDrawableCanvasRenderer.material.SetTextureScale("_MainTex", tiling);

    }
    private void ChangeWhiteboardTextureUVOffset(float m_OffsetgValueX, float m_OffsetgValueY)
    {

        Vector2 offset = new Vector2(m_OffsetgValueX, m_OffsetgValueY);
        m_WBDrawableCanvasRenderer.material.SetTextureOffset("_MainTex", offset);

    }


}
