using MTC.Core.Classes;
using MTC.Utils;
using UnityEngine;

public class Wall : BaseParkingLotObject
{
    public override void PopulateObject(ParkingLotObjectData data)
    {
        base.PopulateObject(data);
        BoxCollider collider = GetComponent<BoxCollider>();

        if (GameManager.Instance != null)
        {
            collider.center = GameManager.GetConfig().wallColliderPosition;
            collider.size = GameManager.GetConfig().wallColliderScale;
        }
    }

    public Wall()
    {
        
    }
}
