using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhiteboardUIPresenter : MonoBehaviour
{

    [SerializeField] private Text WhiteboardLabel;

    public string GetWhiteboardLabel()
    {
        return WhiteboardLabel.text;
    }


}
