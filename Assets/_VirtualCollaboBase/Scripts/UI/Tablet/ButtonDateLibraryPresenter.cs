using System;
using System.Collections;
using System.Collections.Generic;

using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDateLibraryPresenter : MonoBehaviour
{
    [SerializeField] private Text Text_Date;
    [SerializeField] private Text Text_Time;
    [SerializeField] private Text Text_People;
    private static string dateformat = "yyMMddHHmm";
    public string targetDataPath;
    public string dateString;
    public string RoomID;

    private static Subject<ButtonDateLibraryPresenter> m_OnClickDateButton = new Subject<ButtonDateLibraryPresenter>();　///自分自身を送る
    public static IObservable<ButtonDateLibraryPresenter> OnClickDateButton { get { return m_OnClickDateButton; } }


    public void OnClicked()
    {
        m_OnClickDateButton.OnNext(this);
    }

    public void SetData(string date,string roomId)/////ライブラリのボタンにデータをセットして表示
    {
       
            dateString = date;
            DateTime dt = System.DateTime.ParseExact(date, dateformat,  ///1906041135steingはyyMMddHHmmというフォーマットだからdatteTimeに変換してね
            System.Globalization.DateTimeFormatInfo.InvariantInfo,
            System.Globalization.DateTimeStyles.None);

            Text_Date.text = dt.ToString("M/d (ddd)");  ///5/3(火)と表示
            Text_Time.text = dt.ToShortTimeString() + "~";
        //Text_Time.text = dt.ToShortTimeString() + "-XX:XX (XX分) ";
        //Text_People.text = room.NumberOfParticipants + "人参加";
        targetDataPath = roomId + "/" + date;/////TabletforLobbyPresenterに渡すための変数
        RoomID = roomId;
    }

    public string GetDateText()
    {
        return Text_Date.text;
    }
}

