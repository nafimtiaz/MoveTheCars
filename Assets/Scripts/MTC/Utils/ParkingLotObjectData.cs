using System;
using MTC.Core.Enums;
using UnityEngine;

namespace MTC.Utils
{
    [Serializable]
    public class ParkingLotObjectData
    {
        public ParkingLotObjectType lotObjectType;

        public string lotObjectSubType;

        public Vector3 position;

        public Vector3 rotation;
    }
}