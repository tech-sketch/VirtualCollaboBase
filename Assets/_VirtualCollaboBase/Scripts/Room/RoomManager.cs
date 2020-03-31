using System;

namespace Recollab.RoomManagement
{
    public class RoomManager
    {
        public RoomManager()
        {
            //デフォルトルームを仮生成
            CreateRoom("テスト", SystemDefines.SCENE_NAME_ROOM_STAGE_01, "DefaultRoom");
        }

        public void CreateRoom(string roomName, string roomType, string roomId = "")
        {
            Room room = new Room();

            if (string.IsNullOrEmpty(roomId))
            {
                room.RoomId = roomId;
            }
            else
            {
                room.RoomId = Guid.NewGuid().ToString("N").Substring(0, 6);
            }

            room.RoomName = roomName;
            room.RoomStageType = roomType;
            room.LastHeldTimeStart = DateTime.Now.ToString("yyyyMMddHHmm");
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.AddMinutes(30);
            room.LastHeldTimeEnd = dateTime.ToString("yyyyMMddHHmm");
            room.NumberOfTimes = 1;
            room.NumberOfParticipants = 1;
             
            RoomInfo.m_CurrentRoom = room;
            RoomInfo.m_Rooms.Insert(0, room);
        }

        public void CreateJoinedRoomList()
        {
            // ToDo
        }
    }
}