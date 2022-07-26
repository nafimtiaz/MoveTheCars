using MTC.Core.Classes;
using MTC.Core.Enums;
using MTC.Core.Interfaces;
using UnityEngine;
using DG.Tweening;
using MTC.Utils;

public class Vehicle : BaseParkingLotObject, IVehicle
{
    public int VehicleLength { get; set; }
    public VehicleType VehicleType { get; set; }
    private Sequence sequence;

    public override void PopulateObject(ParkingLotObjectData data)
    {
        base.PopulateObject(data);
        BoxCollider collider = GetComponent<BoxCollider>();
        
        collider.center = GameManager.GetConfig().vehicleColliderPosition;
        collider.size = GameManager.GetConfig().vehicleColliderScale;

    }

    /// <summary>
    /// Called when car successfully moves out of the parking lot
    /// </summary>
    public virtual void OnSuccess()
    {
        MakeSuccessSound();
        ShowSuccessEffect();
    }

    public virtual void CheckAndMove(Vector3 dir)
    {
        float crossProd = Mathf.RoundToInt(Vector3.Cross(transform.forward, dir).magnitude);

        if (crossProd >= 1)
        {
            return;
        }
        
        RaycastHit hit;
        Vector3 origin = transform.position + dir + new Vector3(0f, 0.5f, 0f);
        
        if(Physics.Raycast(origin,dir,out hit,Mathf.Infinity))
        {
            float dist = Mathf.RoundToInt(hit.distance);
            float moveDuration = dist / GameManager.GetConfig().vehicleSpeedUnitsPerSecond;
            
            if (hit.transform.GetComponent<IParkingLotObject>() != null)
            {
                transform.DOMove(transform.position + (dir * dist), moveDuration).SetEase(Ease.Linear).OnComplete((() =>
                {
                    if (hit.transform.GetComponent<IParkingLotObject>() != null)
                    {
                        IParkingLotObject lotObject = hit.transform.GetComponent<IParkingLotObject>();
                        OnImpact();
                        lotObject.OnImpact();
                    }
                    else
                    {
                        // TODO: vehicle move out sequence
                        StartEscapeSequence();
                    }
                }));
            }
        }
    }

    private void StartEscapeSequence()
    {
        gameObject.SetActive(false);
    }

    private void MakeSuccessSound()
    {
        
    }

    private void ShowSuccessEffect()
    {
        
    }
}
