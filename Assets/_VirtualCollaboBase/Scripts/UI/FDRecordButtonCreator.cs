using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Recollab.RoomManagement;
using UniRx.Async;
using UnityEngine;
using Zenject;
using UniRx;

public class FDRecordButtonCreator : MonoBehaviour
{
    [SerializeField] private GameObject ButtonLibraryFolderPrefab;
    [SerializeField] private GameObject LibraryFolderHolder;
    [SerializeField] private GameObject ButtonLibraryDatePrefab;
    [SerializeField] private GameObject LibraryDateHolder;

    // Start is called before the first frame update
    void Start()
    {
        InitializeLibraryHolder();
        ButtonFolderLibraryPresenter.OnClickFolderButton
           .Subscribe(x => GetFileList(x))
           .AddTo(this);

        LobbyTabletPresenter.OnRoomInfoLoadComplete
           .Subscribe(x => InitializeLibraryHolder())
           .AddTo(this);
    }

    private void GetFileList(ButtonFolderLibraryPresenter bflp)///日付データのリスト取得
    {
        string roomid = bflp.GetRoomInfo().RoomId;
        GetFileListAsync(roomid).Forget();
    }

    private async UniTask GetFileListAsync(string roomId)
    {
        var list = new ArrayList();

        foreach (var key in list)
        {
            CreateDateRoomButton("test",roomId);
        }

        if (list.Count == 0)
        {
            Debug.Log("File not Found");

        }

    }

    private void CreateDateRoomButton(string date ,string roomId)///第2階層のルームボタンを作る
    {

        GameObject button = Instantiate(ButtonLibraryDatePrefab);
        ButtonDateLibraryPresenter blp = button.GetComponent<ButtonDateLibraryPresenter>();

        blp.SetData(date,roomId);

        button.transform.parent = LibraryDateHolder.transform;
        button.transform.localPosition = Vector3.zero;
        button.transform.localRotation = Quaternion.identity;

    }

    private void InitializeLibraryHolder()///第1階層のボタンを作る
    {
        //foreach (var room in RoomInfo.m_Rooms)
        //{
        //    Debug.Log(room.RoomId);

        //}

        FileIOUtil.SafeCreateDirectory(SystemDefines.FDRECORD_PATH);

        var directories = RoomInfo.m_Rooms;

        int itemCount = 6;

        if (directories.Count <= itemCount)
        {
            itemCount = directories.Count;

        }

        for (int i = 0; i < itemCount; i++)///第一階層
        {
            GameObject button = Instantiate(ButtonLibraryFolderPrefab);
            ButtonFolderLibraryPresenter blp = button.GetComponent<ButtonFolderLibraryPresenter>();
            blp.SetRoomInfo(directories[i]);


            button.transform.parent = LibraryFolderHolder.transform;
            button.transform.localPosition = Vector3.zero;
            button.transform.localRotation = Quaternion.identity;
        }

    }



}


