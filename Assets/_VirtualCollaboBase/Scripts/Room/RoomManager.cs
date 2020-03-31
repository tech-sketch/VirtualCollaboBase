using System;
using UniRx.Async;
using Recollab.UserManagement;

namespace Recollab.RoomManagement
{
    public class RoomManager
    {
        public RoomManager()
        {
            //デフォルトルームを仮生成
            CreateRoom("test", SystemDefines.SCENE_NAME_ROOM_STAGE_01);
        }

        public void CreateRoom(string roomName, string roomType)
        {
          
            Room room = new Room();
            room.RoomId = Guid.NewGuid().ToString("N").Substring(0, 6);
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
            CreateJoinedRoomList();
        }

        public void CreateJoinedRoomList()
        {
           

        }


    }
}