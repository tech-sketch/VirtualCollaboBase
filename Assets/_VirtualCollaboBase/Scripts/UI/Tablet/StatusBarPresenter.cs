using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarPresenter : MonoBehaviour
{

    [SerializeField] private Text Text_Time;
    [SerializeField] private Text Text_Battery;
    [SerializeField] private Text Text_RoomNetwork;
    [SerializeField] private Text Text_PhotonState;
    [SerializeField] private Image Image_WifiState;

    [SerializeField] private Sprite Sprite_WifiOn;
    [SerializeField] private Sprite Sprite_WifiOff;

    private readonly static double STATE_UPDATE_INTERVAL = 5.0f;
    private readonly static float PERCENT_CONVERSION = 100.0f;



    private void Start()
    {
        UpdateStatus();

        Observable.Interval(TimeSpan.FromSeconds(STATE_UPDATE_INTERVAL)).Subscribe(_ => UpdateStatus()).AddTo(this);

    }

    private void UpdateStatus()
    {
        GetClock();
        GetWifiStatus();
        GetBatteryStatus();
        GetPhotonStatus();
 
    }

    private void GetClock()
    {
        Text_Time.text = DateTime.Now.ToShortTimeString();
    }

    private void GetWifiStatus()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:  //切れてる
                Image_WifiState.sprite = Sprite_WifiOff;
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:  //キャリアでつながっている
                Image_WifiState.sprite = Sprite_WifiOn;
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:　//Wifiまたはケーブルでつながっている
                Image_WifiState.sprite = Sprite_WifiOn;
                break;
        }
    }

    private void GetPhotonStatus()
    {
     
        if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
        {
            Text_RoomNetwork.color = Color.black;
            Text_PhotonState.color = Color.black;
            Text_PhotonState.text = "入";
        }
        else
        {
            Text_RoomNetwork.color = new Color(142 / 256f, 142 / 256f, 147 / 256f, 1.0f);
            Text_PhotonState.color = new Color(142 / 256f, 142 / 256f, 147 / 256f, 1.0f);
            Text_PhotonState.text = "切";
        }
    }

    private void GetBatteryStatus()
    {
        int battery = (int)(SystemInfo.batteryLevel * PERCENT_CONVERSION);
        Text_Battery.text = battery.ToString()　+ "%";
    }
}
