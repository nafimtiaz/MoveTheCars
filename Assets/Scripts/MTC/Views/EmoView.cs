using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class EmoView : MonoBehaviour
{
    [SerializeField] private GameObject[] positiveEmos;
    [SerializeField] private GameObject[] negativeEmos;
    private Transform targetVehicle;
    private Vector3 offsetHeight = new Vector3(0f, 1f, 0f);
    private Sequence sequence;
    private GameObject currentEmo;


    public void TriggerEmo(Transform target, bool isPositive)
    {
        sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            UpdateEmoBubblePosition(target);
            gameObject.SetActive(true);
            int emoIndex = Random.Range(0, 5);
            currentEmo = isPositive ? positiveEmos[emoIndex] : negativeEmos[emoIndex];
            currentEmo.SetActive(true);
            targetVehicle = target;
        });
        sequence.AppendInterval(1f);
        sequence.AppendCallback(() =>
        {
            currentEmo.SetActive(false);
            targetVehicle = null;
            gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        if (sequence != null)
        {
            sequence.Kill();
        }
    }

    private void Update()
    {
        if (targetVehicle != null)
        {
            UpdateEmoBubblePosition(targetVehicle);
        }
    }

    private void UpdateEmoBubblePosition(Transform target)
    {
        Vector3 pos = target.transform.position + offsetHeight;
        transform.position = Camera.main.WorldToScreenPoint(pos);
    }
}
