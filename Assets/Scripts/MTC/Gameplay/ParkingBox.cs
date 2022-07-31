using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ParkingBox : MonoBehaviour
{
    [SerializeField] private Transform bar;
    private int vehicleCount = 0;
    
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("vehicle"))
        {
            if (vehicleCount == 0)
            {
                bar.DOLocalRotate(new Vector3(0f, 0f, -70f), 0.1f);
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
                bar.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.1f);
            }
        }
    }
}
