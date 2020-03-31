using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Recollab.RoomManagement;
using System;
using Zenject;
using Recollab.Network;
using UniRx;

public class RoomInfoViewer : MonoBehaviour
{
    /// <summary>
    /// このスクリプトはRoomTabletresenterに移植したのち消す
    /// </summary>
    [SerializeField] private Text RoomNameText;
    [SerializeField] private Text RoomIDText;
    [SerializeField] private Text LastHeldTimeText;
    [SerializeField] private Text NumberOfTimes;
    [SerializeField] private Text NumberOfParticipantsText;
    private string dateformat = "yyyyMMddHHmm";
    [Inject]
    private void Initialize( INetworkEngineConnector manager)
    {
        manager.OnJoinedRoomAsObservable.Subscribe(_ => ShowInfo()).AddTo(this);
    }

    public void ShowInfo()
    {
        Room room = RoomInfo.m_CurrentRoom;

        DateTime dtStart = DateTime.ParseExact(room.LastHeldTimeStart, dateformat,  ///1906041135はyyMMddHHmmというフォーマットだからdatteTimeに変換してね
        System.Globalization.DateTimeFormatInfo.InvariantInfo,
        System.Globalization.DateTimeStyles.None);

        DateTime dtEnd = DateTime.ParseExact(room.LastHeldTimeEnd, dateformat,  ///1906041135はyyMMddHHmmというフォーマットだからdatteTimeに変換してね
        System.Globalization.DateTimeFormatInfo.InvariantInfo,
        System.Globalization.DateTimeStyles.None);

        string LastHeldTime = dtStart.ToShortDateString() + dtStart.ToString("(ddd)") + dtStart.ToShortTimeString() + "-" + dtEnd.ToShortTimeString();
        Debug.Log(LastHeldTime);

        RoomIDText.text = room.RoomId;
        RoomNameText.text = room.RoomName;
        LastHeldTimeText.text = LastHeldTime;
        NumberOfTimes.text = room.NumberOfTimes.ToString();
        NumberOfParticipantsText.text = room.NumberOfParticipants.ToString();
    }
}
