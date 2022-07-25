using MTC.Core.Enums;
using UnityEngine;

namespace MTC.Core.Interfaces
{
    public interface IVehicle
    {
        public int VehicleLength { get; set; }

        public VehicleType VehicleType { get; set; }

        public void OnSuccess();
        
        public void CheckAndMove(Vector3 dir);
    }   
}
