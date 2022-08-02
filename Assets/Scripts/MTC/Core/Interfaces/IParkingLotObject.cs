using MTC.Core.Enums;
using UnityEngine;

namespace MTC.Core.Interfaces
{
    public interface IParkingLotObject
    {
        public ParkingLotObjectType LotObjectType { get; set; }

        public string LotObjectSubType { get; set; }

        public Vector3 Position { get; set; }
        
        public Vector3 Rotation { get; set; }
        
        void OnImpact(Vector3 hitPoint, bool isHitter);
    }
}