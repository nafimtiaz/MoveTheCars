using MTC.Core.Enums;
using UnityEngine;

namespace MTC.Utils
{
    [System.Serializable]
    public class ParkingLotObjectData
    {
        public ParkingLotObjectType lotObjectType;

        public string lotObjectSubType;

        public Vector3 position;

        public Vector3 rotation;

        public ParkingLotObjectData(ParkingLotObjectType type, string subType, Vector3 pos, Vector3 rot)
        {
            lotObjectType = type;
            lotObjectSubType = subType;
            position = pos;
            rotation = rot;
        }
    }
}