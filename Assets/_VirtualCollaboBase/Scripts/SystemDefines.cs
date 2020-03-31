using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SystemDefines
{


    /////////////////////////
    public readonly static string USER_RECORD_PATH = System.IO.Directory.GetCurrentDirectory() + "/Record";
    public readonly static string USER_VOICE_RECORD_PATH = USER_RECORD_PATH + "/Voice/";
    public readonly static string USER_DRAWING_RECORD_PATH = USER_RECORD_PATH + "/Drawing/";
    public readonly static string USER_EVENTLOG_RECORD_PATH = USER_RECORD_PATH + "/EventLog/";


    /////////////////////////
    public readonly static string FDRECORD_RELATIVE_PATH = "/ObjRecord/";
    public readonly static string FDRECORD_RELATIVE_PATH2 = "ObjRecord/";
    public readonly static string FDRECORD_PATH = Application.streamingAssetsPath + FDRECORD_RELATIVE_PATH;
    public readonly static string SNAPSHOT_ROOM_RELATIVE_PATH = "/Snapshot/Room/";
    public readonly static string SNAPSHOT_ROOM_PATH = USER_RECORD_PATH + SNAPSHOT_ROOM_RELATIVE_PATH;
    public readonly static string SNAPSHOT_ROOM_INFO_FILENAME = "SnapshotRoomInfo.roomInfo";
    public readonly static string PLAYABLE_AVATAR_PATH = "Avatars/Playable/";
	public readonly static string FDRECORD_DRAWING_DIR_NAME = "Drawing";
	public readonly static string FDRECORD_DRAWING_PREFIX = "Drawing_";
	public readonly static string FDRECORD_DRAWING_SUFFIX = ".rec";
    public readonly static string FDRECORD_DATA_SUFFIX = ".zip";
    public readonly static string SNAPSHOT_WHITEBOARD_DIR_NAME ="Whiteboard";
    public readonly static string SNAPSHOT_STICKY_DIR_NAME = "Sticky";
    public readonly static int STICKY_TEXTURE_WIDTH = 512;
    public readonly static int STICKY_TEXTURE_HEIGHT = 512;
    public readonly static int WHITEBOARD_TEXTURE_WIDTH = 1280;
    public readonly static int WHITEBOARD_TEXTURE_HEIGHT = 800;

	public readonly static string SCENE_NAME_CORE = "Core";
	public readonly static string SCENE_NAME_LOBBY = "Lobby";
	public readonly static string SCENE_NAME_LOBBY_STAGE_01 = "LobbyStage_01";
	public readonly static string SCENE_NAME_LOBBY_STAGE_02 = "LobbyStage_02";
	public readonly static string SCENE_NAME_ROOM = "Room";
	public readonly static string SCENE_NAME_ROOM_STAGE_01 = "RoomStage_01";
	public readonly static string SCENE_NAME_ROOM_STAGE_02 = "RoomStage_02";

    public readonly static string LOGIN_USER_FILE = Application.persistentDataPath + "/user.login";



}
