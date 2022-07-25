using DG.Tweening;
using MTC.Core.Enums;
using MTC.Core.Interfaces;
using MTC.Utils;
using UnityEngine;

namespace MTC.Core.Classes
{
    public class BaseParkingLotObject : MonoBehaviour, IParkingLotObject
    {
        public ParkingLotObjectType LotObjectType { get; set; }
        public string LotObjectSubType { get; set; }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        public Vector3 Rotation
        {
            get => transform.eulerAngles;
            set => transform.eulerAngles = value;
        }
        
        /// <summary>
        /// This function places the object in the lot
        /// also sets some properties
        /// </summary>
        /// <param name="data">Parking Lot Object data</param>
        public virtual void PopulateObject(ParkingLotObjectData data)
        {
            Position = data.position;
            Rotation = data.rotation;
            LotObjectType = data.lotObjectType;
            LotObjectSubType = data.lotObjectSubType;
        }
        
        /// <summary>
        /// Called upon impact
        /// </summary>
        public virtual void OnImpact()
        {
            MakeImpactVibration();
            MakeImpactSound();
        }

        private void MakeImpactSound()
        {
            
        }
        
        private void MakeImpactVibration()
        {
            transform.DOShakePosition(0.3f, 0.1f, 15, 0f);
        }
    }
}