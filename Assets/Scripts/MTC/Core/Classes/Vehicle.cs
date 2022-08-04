using System;
using MTC.Core.Classes;
using MTC.Core.Enums;
using MTC.Core.Interfaces;
using UnityEngine;
using DG.Tweening;
using MTC.Utils;
using Random = UnityEngine.Random;

public class Vehicle : BaseParkingLotObject, IVehicle
{
    public int VehicleLength { get; set; }
    public VehicleType VehicleType { get; set; }

    public bool isInteractable;
    private bool isMovingFwd;
    private bool isTurning;
    private Sequence sequence;
    private Vector3 axisOrigin;
    private Vector3 axisDir;
    private float turningTime;
    private float linearSpeed;
    private bool isConsecutiveTurn;
    public bool hasLeftParkingLot => isMovingFwd || isTurning;

    private Vector3 checkPos;

    public override void PopulateObject(ParkingLotObjectData data)
    {
        base.PopulateObject(data);
        BoxCollider collider = GetComponent<BoxCollider>();
        
        collider.center = GameManager.GetConfig().vehicleColliderPosition;
        collider.size = GameManager.GetConfig().vehicleColliderScale;

        isTurning = false;
        isMovingFwd = false;
        isConsecutiveTurn = false;
        isInteractable = true;

        if (GetComponent<Rigidbody>() != null)
        {
            Destroy(GetComponent<Rigidbody>());
        }
        
        if (sequence != null)
        {
            sequence.Kill();
        }
    }

    #region Vehicle Interaction

    public virtual void CheckAndMove(Vector3 dir)
    {
        if (!isInteractable)
        {
            return;
        }
        
        // accept touches only along vehicle's forward or backward direction
        float crossProd = Mathf.RoundToInt(Vector3.Cross(transform.forward, dir).magnitude);

        if (crossProd >= 1)
        {
            return;
        }
        
        if (sequence != null)
        {
            sequence.Kill();
        }
        
        
        // If the vehicle finds a lot object towards swipe direction,
        // then it will move to it and call impact effect, otherwise
        // it will consider the move as an escape from parking lot
        // and start escape sequence
        RaycastHit hit;
        Vector3 origin = transform.position + dir + new Vector3(0f, 0.5f, 0f);
        
        if(Physics.Raycast(origin,dir,out hit,Mathf.Infinity))
        {
            isInteractable = false;
            bool isEscapeRoute = hit.transform.GetComponent<IParkingLotObject>() == null || isRunningCar(hit.transform);
            float dist = Mathf.RoundToInt(hit.distance) + (isEscapeRoute ? -1f : 0);
            float moveDuration = dist / GameManager.GetConfig().vehicleSpeedUnitsPerSecond;
            
            transform.DOMove(transform.position + (dir * dist), moveDuration).SetEase(Ease.Linear).OnComplete((() =>
            {
                if (!isEscapeRoute)
                {
                    IParkingLotObject lotObject = hit.transform.GetComponent<IParkingLotObject>();
                    OnImpact(hit.point, true);
                    lotObject.OnImpact(Vector3.zero, false);
                    isInteractable = true;
                }
                else
                {
                    // check if vehicle is moving from front or back
                    bool isBackward = Vector3.Dot(transform.forward, dir) < 0;
                    StartEscapeSequence(isBackward);
                }
            }));
        }
    }

    private bool isRunningCar(Transform car)
    {
        Vehicle vehicle = car.GetComponent<Vehicle>();
        if (vehicle == null)
        {
            return false;
        }

        return vehicle.hasLeftParkingLot;
    }

    // the vehicle which gets hit gets annoyed, not the hitter
    public override void OnImpact(Vector3 hitPoint, bool isHitter)
    {
        if (isInteractable || isHitter)
        {
            base.OnImpact(hitPoint, isHitter);
        }

        if (isHitter)
        {
            GameObject.Instantiate(GameManager.GetConfig().impactParticle, hitPoint, Quaternion.identity);
            GameManager.GetSoundManager().PlaySound(GameManager.GetConfig().impactSound);
        }
        else
        {
            GameManager.GetHomeView().AssignEmoBubble(transform,false);
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
        Vector3 checkDir = isBackward ? (-1f * transform.forward) : transform.forward;
        Vector3 origin = transform.position + (checkDir * 2f) + new Vector3(0f, 0.5f, 0f);
        
        Collider[] cols = Physics.OverlapBox(origin,
            new Vector3(0.5f, 0.5f, 2f), 
            Quaternion.Euler(transform.eulerAngles + new Vector3(0f,90f,0f)),
            GameManager.GetConfig().vehicleLayerMask);

        if (cols.Length > 0)
        {
            // Vehicle checks if it will collide with
            // any incoming vehicle, if no then it comes out of the lot
            // and starts running
            isInteractable = true;
            GameManager.GetHomeView().AssignEmoBubble(transform,false);
            sequence = DOTween.Sequence();
            sequence.AppendInterval(1f).OnComplete(() =>
            {
                isInteractable = false;
                StartEscapeSequence(isBackward);
            });
        }
        else
        {
            isInteractable = false;
            float moveDuration = 1f / GameManager.GetConfig().vehicleSpeedUnitsPerSecond;
            transform.DOMove(transform.position + checkDir, moveDuration).SetEase(Ease.Linear).OnComplete((() =>
            {
                GameManager.GetSoundManager().PlaySound(GameManager.GetConfig().carSound1);
                TriggerTurnDirection(false, isBackward);
            }));
        }
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
                    GameManager.GetSoundManager().PlaySound(GameManager.GetConfig().carSound2);
                }
                else
                {
                    TriggerTurnDirection(false, false);
                }
            });
        }
        // this is an edge case where vehicles have to 
        // do back to back turning, common for vehicles at corner positions
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
        // colliders with cornerRight and cornerLeft helps the vehicle
        // to change direction while doing escape sequence
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
        }
        
        // return the object to the pool
        if (col.CompareTag("finish"))
        {
            isMovingFwd = false;
            isTurning = false;
            isConsecutiveTurn = false;
            isInteractable = false;
            sequence.Kill();
            GameManager.GetPool().ReturnObjectToPool(this.gameObject);
        }
        
        // when vehicle reaches the finish line
        if (col.CompareTag("success"))
        {
            OnSuccess();
        }
    }

    #endregion

    /// <summary>
    /// Called when car successfully moves out of the parking lot
    /// </summary>
    public virtual void OnSuccess()
    {
        GameManager.GetHomeView().AddVehicleCount();
        GameManager.GetHomeView().AssignEmoBubble(transform,true);
        MakeSuccessSound();
        ShowSuccessEffect();
    }
    
    private void MakeSuccessSound()
    {
        GameManager.GetSoundManager().PlaySound(GameManager.GetConfig().scoreSound);
    }

    private void ShowSuccessEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = transform.position;
            GameObject.Instantiate(GameManager.GetConfig().celebrationParticle,
                new Vector3(pos.x + Random.Range(-2f, 2f),
                    pos.y + Random.Range(2f, 4f),
                    pos.z + Random.Range(-2f, 2f)), Quaternion.identity);
        }
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
