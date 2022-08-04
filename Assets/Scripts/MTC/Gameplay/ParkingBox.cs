using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ParkingBox : MonoBehaviour
{
    [SerializeField] private Transform bar;
    private int vehicleCount = 0;
    private Vector3 openRot = new Vector3(0f, 0f, -70f);
    
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("vehicle"))
        {
            if (vehicleCount == 0)
            {
                bar.DOLocalRotate(openRot, 0.1f);
            }
            
            vehicleCount++;
        }
    }
    
    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("vehicle"))
        {
            vehicleCount--;
            
            if (vehicleCount == 0)
            {
                bar.DOLocalRotate(Vector3.zero, 0.1f);
            }
        }
    }

    public void ResetParkingBox()
    {
        vehicleCount = 0;
        bar.localEulerAngles = Vector3.zero;
    }
}
