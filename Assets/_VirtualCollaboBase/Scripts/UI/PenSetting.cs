using SimpleDrawing.Sharing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using VRWS.SimpleDrawing.Sharing;
using System;

public class PenSetting : MonoBehaviour
{
    public RemoteRayCastDrawer remoteRayCastDrawer;

    void Start()
    {
        SimpleDrawingSharingInitializer.OnPenSpawn.Subscribe(x => Initialize(x)).AddTo(this);
    }

    private void Initialize(RemoteRayCastDrawer ray)
    {
        remoteRayCastDrawer = ray;
    }

    public void OnPensizeBig()
    {
        remoteRayCastDrawer.PenWidth = 10;
    }

    public void OnPensizeMiddle()
    {
        remoteRayCastDrawer.PenWidth = 4;
    }

    public void OnPensizeSmall()
    {
        remoteRayCastDrawer.PenWidth = 1;
    }

    public void OnPenColorGreen()
    {
        remoteRayCastDrawer.PenColor = new Color32(76, 175, 80, 255); ;
        remoteRayCastDrawer.Erase = false;
    }

    public void OnPenColorRed()
    {
        remoteRayCastDrawer.PenColor = new Color32(223, 44, 44, 255); ;
        remoteRayCastDrawer.Erase = false;
    }

    public void OnPenColorBlack()
    {
        remoteRayCastDrawer.PenColor = new Color32(41, 41, 41, 255); ;
        remoteRayCastDrawer.Erase = false;
    }

    public void OnPenColorBlue()
    {
        remoteRayCastDrawer.PenColor = new Color32(0, 122, 255, 255); ;
        remoteRayCastDrawer.Erase = false;
    }

    public void OnPenErase()
    {
        remoteRayCastDrawer.Erase = true ;
    }
}
