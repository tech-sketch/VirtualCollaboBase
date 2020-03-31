using System;
using System.Collections.Generic;

namespace Recollab.RoomManagement
{
    [Serializable]
    public class Room : IEquatable<Room>
    {
        public string RoomId;
        public string RoomName;
        public string RoomStageType;
        public string LastHeldTimeStart;
        public string LastHeldTimeEnd;
        public int NumberOfTimes;
        public int NumberOfParticipants;

        public bool Equals(Room other)
        {
            return (this.RoomId.Equals(other.RoomId));
        }
    }

    [Serializable]
    public class JoinedRoomList
    {
        public string UserID;
        public List<Room> Rooms;
    }

}

