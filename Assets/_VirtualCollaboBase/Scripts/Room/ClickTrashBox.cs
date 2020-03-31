using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ClickTrashBox : MonoBehaviour
{
    private static Subject<Unit> m_OnClickTrashBox = new Subject<Unit>();
    public static IObservable<Unit> OnClickTrashBox { get { return m_OnClickTrashBox; } }

    public void OnClickdTrashBox()
    {
        m_OnClickTrashBox.OnNext(Unit.Default);
    }
}
