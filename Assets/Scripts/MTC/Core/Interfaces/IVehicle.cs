using MTC.Core.Enums;

namespace MTC.Core.Interfaces
{
    public interface IVehicle
    {
        public int VehicleLength { get; set; }

        public VehicleType VehicleType { get; set; }

        public void OnSuccess();
    }   
}
