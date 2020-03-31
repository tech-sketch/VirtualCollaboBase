using Recollab.RoomManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomSelectButtonPresenter : MonoBehaviour, ISelectHandler,IDeselectHandler
{

    private static Subject<RoomSelectButtonPresenter> m_OnClick = new Subject<RoomSelectButtonPresenter>();　///自分自身を送る
    public static IObservable<RoomSelectButtonPresenter> OnClick { get { return m_OnClick; } }
    private Room m_Room;
    private static string dateformat = "yyyyMMddHHmm";
    [SerializeField] private Button Button_room1;
    [SerializeField] private GameObject SelectingHighLight;
    [SerializeField] private Text Text_roomTitle;
    [SerializeField] private Text Text_daytime;
    [SerializeField] private Text Text_people;


    void Start()
    {
        SetRoomInfo(RoomInfo.m_CurrentRoom);
    }

    public void OnClicked()
    {
        Button_room1.Select();
        m_OnClick.OnNext(this);
    }



    public void SetRoomInfo(Room room)
    {
        m_Room = room;
        Text_roomTitle.text = room.RoomName;
        DateTime dtStart = System.DateTime.ParseExact(room.LastHeldTimeStart, dateformat, System.Globalization.DateTimeFormatInfo.InvariantInfo,System.Globalization.DateTimeStyles.None);
        DateTime dtEnd = System.DateTime.ParseExact(room.LastHeldTimeEnd, dateformat, System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None);


        Text_daytime.text = dtStart.ToString("M/d (ddd)") + " " + dtStart.ToShortTimeString() + "-" + dtEnd.ToShortTimeString();　　///5/3(火)と表示

        Text_people.text = room.NumberOfTimes + "回実施、" + room.NumberOfParticipants + "人参加" ;
    }

    public Room GetRoomInfo()
    {
        return m_Room;
    }

    public void OnSelect(BaseEventData eventData)
    {
        SelectingHighLight.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SelectingHighLight.SetActive(false);
    }
}
