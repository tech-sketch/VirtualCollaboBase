using System;
using System.Collections;
using System.Collections.Generic;
using Recollab.RoomManagement;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLibraryPresenter : MonoBehaviour
{
    [SerializeField] private Text Datetext;
    [SerializeField] private Text Timetext;
    private static string dateformat = "yyMMddHHmm";
    public string targetDataPath;
    public string dateString;
    public Room RoomInfo;
    private static Subject<ButtonLibraryPresenter> m_OnClick = new Subject<ButtonLibraryPresenter>();　///自分自身を送る
    public static IObservable<ButtonLibraryPresenter> OnClick { get { return m_OnClick; } }


    public void onClick()
    {
        m_OnClick.OnNext(this);
    }

    public void SetData(string date)/////ライブラリのボタンにデータをセットして表示
    {
        dateString = date;
        string roomInfoFilePath = SystemDefines.FDRECORD_PATH + date + "/" + SystemDefines.SNAPSHOT_ROOM_INFO_FILENAME;
        //if (System.IO.File.Exists(roomInfoFilePath) == true)
        //{
            Room room = JsonHelper<Room>.Read(roomInfoFilePath);
            RoomInfo = room;
            DateTime dt = System.DateTime.ParseExact(date, dateformat,  ///1906041135steingはyyMMddHHmmというフォーマットだからdatteTimeに変換してね
            System.Globalization.DateTimeFormatInfo.InvariantInfo,
            System.Globalization.DateTimeStyles.None);

            Datetext.text = dt.ToString("M/d (ddd)");  ///5/3(火)と表示
            Timetext.text = dt.ToShortTimeString() + "-XX:XX (XX分) ";
            targetDataPath = RoomInfo.RoomId + "/" + date;/////1906041135みたいなデータをTabletforLobbyPresenterに渡すための変数

        //}
        //else
        //{
        //    //もしルームの情報が取れなければボタンを破壊
        //    Destroy(gameObject);
        //    return;
        //}
        


    }

    public string GetDateText()
    {
        return Datetext.text;
    }

}
