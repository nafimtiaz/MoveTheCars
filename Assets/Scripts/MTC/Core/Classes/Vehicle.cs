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
    
    public bool isMovingFwd;
    public bool isTurning;
    private Sequence sequence;
    private Vector3 axisOrigin;
    private Vector3 axisDir;
    private float turningTime;
    private float linearSpeed;
    public bool isConsecutiveTurn;

    public override void PopulateObject(ParkingLotObjectData data)
    {
        base.PopulateObject(data);
        BoxCollider collider = GetComponent<BoxCollider>();
        
        collider.center = GameManager.GetConfig().vehicleColliderPosition;
        collider.size = GameManager.GetConfig().vehicleColliderScale;

        isTurning = false;
        isMovingFwd = false;
        isConsecutiveTurn = false;
    }

    #region Vehicle Interaction

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
                    bool isBackward = Vector3.Dot(transform.forward, dir) < 0;
                    StartEscapeSequence(isBackward);
                }
            }));
        }
    }

    #endregion

    #region Escape
    
    void Awake()
    {
        turningTime = GameManager.GetConfig().vehicleTurningTime;
        linearSpeed = GameManager.GetConfig().vehicleLinearSpeed;
    }

    private void Update()
    {
        if(isTurning)
        {
            transform.RotateAround(axisOrigin, axisDir, 90f * (1f/turningTime) * Time.deltaTime);
        }

        if (!isTurning && isMovingFwd)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * linearSpeed);
        }
    }

    private void StartEscapeSequence(bool isBackward)
    {
        TriggerTurnDirection(false, isBackward);
    }
    
    /// <summary>
    /// Turns the vehicle to left or right direction by 90 degrees 
    /// </summary>
    private void TriggerTurnDirection(bool isLeft, bool isBackward)
    {
        sequence = DOTween.Sequence();

        if (!isConsecutiveTurn)
        {
            sequence.AppendCallback(() =>
            {
                axisOrigin = transform.position + (isLeft ? (-1f * transform.right) : transform.right);
                axisDir = isLeft ? Vector3.down : Vector3.up;
                axisDir = isBackward ? (-1f * axisDir) : axisDir;
                isTurning = true;
            });
            sequence.AppendInterval(turningTime);
            sequence.AppendCallback(() => isTurning = false).OnComplete(() =>
            {
                if (!isConsecutiveTurn)
                {
                    transform.localPosition = RoundVectorByInterval(transform.localPosition,0.5f);
                    transform.localEulerAngles = RoundVectorByInterval(transform.localEulerAngles,90f);
                    isMovingFwd = true;
                }
                else
                {
                    TriggerTurnDirection(false, false);
                }
            });
        }
        else
        {
            isConsecutiveTurn = false;
            sequence.AppendCallback(() => isTurning = true);
            sequence.AppendInterval(turningTime);
            sequence.AppendCallback(() => isTurning = false).OnComplete(() =>
            {
                transform.localPosition = RoundVectorByInterval(transform.localPosition,0.5f);
                transform.localEulerAngles = RoundVectorByInterval(transform.localEulerAngles,90f);
                isMovingFwd = true;
            });
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("cornerRight"))
        {
            if (Vector3.Dot(transform.forward, (transform.position - col.transform.position).normalized) < 0)
            {
                isMovingFwd = false;
                if (isTurning)
                {
                    isConsecutiveTurn = true;
                }
                else
                {
                    TriggerTurnDirection(false, false);
                }
            }
        }
        
        if (col.CompareTag("cornerLeft"))
        {
            isMovingFwd = false;
            TriggerTurnDirection(true, false);
            OnSuccess();
        }
        
        if (col.CompareTag("finish"))
        {
            isMovingFwd = false;
            isTurning = false;
            sequence.Kill();
            GameManager.GetPool().ReturnObjectToPool(this.gameObject);
        }
    }

    #endregion

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
    
    /// <summary>
    /// This function snaps the Vector3 in given interval. 
    /// example: if interval = 90, converts 91.3 or 88.7 to 90 
    /// </summary>
    private Vector3 RoundVectorByInterval(Vector3 v, float interval)
    {
        return new Vector3(
            Mathf.RoundToInt(v.x/interval) * interval,
            Mathf.RoundToInt(v.y/interval) * interval,
            Mathf.RoundToInt(v.z/interval) * interval);
    }
}
