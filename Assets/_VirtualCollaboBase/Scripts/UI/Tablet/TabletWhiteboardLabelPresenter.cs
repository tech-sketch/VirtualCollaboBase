using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TabletWhiteboardLabelPresenter : MonoBehaviour
{

    [SerializeField] private Text TabletWhiteboardLabel;

    // Start is called before the first frame update
    void Start()
    {
        WhiteBoard.OnClickWhiteBoard.Subscribe(go => SetTabletWhiteboardLabel(go)).AddTo(this);
    }

    private void SetTabletWhiteboardLabel(GameObject whiteboard)
    {
        TabletWhiteboardLabel.text = whiteboard.GetComponent<WhiteboardUIPresenter>().GetWhiteboardLabel();
    }
}
