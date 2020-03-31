using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using SimpleDrawing.Sharing;

public class ControllerHaptics : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        RemoteRayCastDrawer.OnDrawStart.Subscribe(_ => Haptics()).AddTo(this);
        RemoteRayCastDrawer.OnDrawEnd.Subscribe(_ => HapticsEnd()).AddTo(this);
    }

    private void Haptics()
    {
        OVRInput.SetControllerVibration(0.1f, 0.1f, OVRInput.Controller.RTouch);
        
    }

    private void HapticsEnd()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
