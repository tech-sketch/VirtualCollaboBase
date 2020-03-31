using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class WhiteboardClearButtonEvent : MonoBehaviour
{
    private static Subject<Unit> m_OnWhiteboardClearButtonPressed = new Subject<Unit>();
    public static IObservable<Unit> OnWhiteboardClearButtonPressed { get { return m_OnWhiteboardClearButtonPressed; } }

    public void OnWhiteboardClearButtonPress()
    {
        m_OnWhiteboardClearButtonPressed.OnNext(Unit.Default);
    }


}
