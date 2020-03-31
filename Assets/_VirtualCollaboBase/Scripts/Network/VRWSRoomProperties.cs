using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;

public static class VRWSRoomProperties 
{
    private const string KeyDisplayName = "DisplayName"; // ルーム名のキーの文字列
    private const string KeyStartTime = "StartTime"; // ゲーム開始時刻のキーの文字列

    private static Hashtable hashtable = new Hashtable();

    // ルームの初期設定を作成する
    public static RoomOptions CreateRoomOptions(string displayName, byte MaxPlayers)
    {
        return new RoomOptions()
        {
            MaxPlayers = MaxPlayers,
            PublishUserId = true,
            // カスタムプロパティの初期設定
            CustomRoomProperties = new Hashtable() {
                { KeyDisplayName, displayName }
            },
            // ロビーからカスタムプロパティを取得できるようにする
            CustomRoomPropertiesForLobby = new string[] {
                KeyDisplayName
            }
        };
    }

    // ルーム名を取得する
    public static string GetDisplayName(this Room room)
    {
        return (string)room.CustomProperties[KeyDisplayName];
    }

    // ゲーム開始時刻が設定されているか調べる
    public static bool HasStartTime(this Room room)
    {
        return room.CustomProperties.ContainsKey(KeyStartTime);
    }

    // ゲーム開始時刻があれば取得する
    public static bool TryGetStartTime(this Room room, out int timestamp)
    {
        bool result = room.CustomProperties.TryGetValue(KeyStartTime, out var value);
        timestamp = (result) ? (int)value : 0;
        return result;
    }

    // ゲーム開始時刻を設定する
    public static void SetStartTime(this Room room, int timestamp)
    {
        hashtable[KeyStartTime] = timestamp;

        room.SetCustomProperties(hashtable);
        hashtable.Clear();
    }


}
