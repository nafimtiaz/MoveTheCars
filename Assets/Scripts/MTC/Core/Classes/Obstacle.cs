using MTC.Core.Classes;
using MTC.Utils;
using UnityEngine;

public class Obstacle : BaseParkingLotObject
{
    [SerializeField] Transform obstacleMeshTransform;
    
    public override void PopulateObject(ParkingLotObjectData data)
    {
        base.PopulateObject(data);
        obstacleMeshTransform.localEulerAngles = new Vector3(0f, 90f * Random.Range(0f, 5f), 0f);
    }
}
