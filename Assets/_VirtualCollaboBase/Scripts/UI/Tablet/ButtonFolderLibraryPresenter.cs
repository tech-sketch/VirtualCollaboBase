using System;
using System.Collections;
using System.Collections.Generic;
using Recollab.RoomManagement;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
public class ButtonFolderLibraryPresenter : MonoBehaviour
{
    [SerializeField] private Text Text_roomTitle;

    private Room m_Room;
    private static Subject<ButtonFolderLibraryPresenter> m_OnClickFolderButton = new Subject<ButtonFolderLibraryPresenter>();　///自分自身を送る
    public static IObservable<ButtonFolderLibraryPresenter> OnClickFolderButton { get { return m_OnClickFolderButton; } }


    public void OnClicked()
    {
        m_OnClickFolderButton.OnNext(this);
    }

    public void SetRoomInfo(Room room)
    {
        m_Room = room;
        Text_roomTitle.text = room.RoomName;
    }

    public Room GetRoomInfo()
    {
        return m_Room;
    }

}