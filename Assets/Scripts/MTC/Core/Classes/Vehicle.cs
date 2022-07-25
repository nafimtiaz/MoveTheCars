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
            
            if (hit.transform.GetComponent<IParkingLotObject>() != null)
            {
                transform.DOMove(transform.position + (dir * dist), dist * 0.1f).SetEase(Ease.Linear).OnComplete((() =>
                {
                    IParkingLotObject lotObject = hit.transform.GetComponent<IParkingLotObject>();
                    OnImpact();
                    lotObject.OnImpact();
                }));
            }
            else
            {
                transform.DOMove(transform.position + (dir * dist), dist * 0.1f).SetEase(Ease.Linear).OnComplete((() =>
                {
                    // TODO: vehicle move out sequence
                    gameObject.SetActive(false);
                }));
            }
        }
    }

    private void MakeSuccessSound()
    {
        
    }

    private void ShowSuccessEffect()
    {
        
    }
}
