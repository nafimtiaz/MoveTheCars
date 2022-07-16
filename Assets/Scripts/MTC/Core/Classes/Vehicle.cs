using MTC.Core.Classes;
using MTC.Core.Enums;
using MTC.Core.Interfaces;

public class Vehicle : BaseParkingLotObject, IVehicle
{
    public int VehicleLength { get; set; }
    public VehicleType VehicleType { get; set; }
    
    /// <summary>
    /// Called when car successfully moves out of the parking lot
    /// </summary>
    public virtual void OnSuccess()
    {
        MakeSuccessSound();
        ShowSuccessEffect();
    }

    private void MakeSuccessSound()
    {
        
    }

    private void ShowSuccessEffect()
    {
        
    }
}
