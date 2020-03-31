using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using SimpleDrawing;
using UnityEngine.UI;

public class HandWritingStickeyController : MonoBehaviour
{
    // Start is called before the first frame update
    private static Subject<GameObject> m_OnHandWritingStickeyClick = new Subject<GameObject>();
    public static IObservable<GameObject> OnHandWritingStickeyClick { get { return m_OnHandWritingStickeyClick; } }

    private static Subject<Texture2D> m_OnHandWritingStickeyComplete = new Subject<Texture2D>();
    public static IObservable<Texture2D> OnHandWritingStickeyComplete { get { return m_OnHandWritingStickeyComplete; } }

    public GameObject HandWritingStickeyCanvas;

    void Start()
    {
        
    }

public void OnHandWritingStickeyButtonClick()
    {

        m_OnHandWritingStickeyClick.OnNext(HandWritingStickeyCanvas);
    }

    public void OnHandWritingStickeySave()
    {
        DrawableCanvas drawableCanvas= HandWritingStickeyCanvas.GetComponent<DrawableCanvas>();
        m_OnHandWritingStickeyComplete.OnNext(drawableCanvas.GetTexture2D());
        drawableCanvas.ResetCanvas();
    }

    public void OnHandWritingStickeyReset()
    {
        DrawableCanvas drawableCanvas = HandWritingStickeyCanvas.GetComponent<DrawableCanvas>();
        drawableCanvas.ResetCanvas();
    }
}
