using System;
using MTC.Core.Classes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MTC.Utils
{
    public static class ObjectCreator
    {
        /// <summary>
        /// Creates and places a parking lot object in scene
        /// </summary>
        /// <param name="data">ParkingLotObjectData of an object in a level retrieved from json</param>
        /// <returns>the BaseParkingLotObject that has been created</returns>
        public static BaseParkingLotObject CreateAndPlaceObject(ParkingLotObjectData data, bool fromPool = false)
        {
            GameObject lotGameObject;
            
            try
            {
                if (!fromPool)
                {
                    var assetPath = $"Objects/{data.lotObjectType.ToString()}/{data.lotObjectSubType}";
                    lotGameObject = Object.Instantiate(Resources.Load(assetPath)) as GameObject;
                }
                else
                {
                    lotGameObject = GameManager.GetPool().GetObjectFromPool(data.lotObjectSubType);
                    lotGameObject.SetActive(true);
                }

                if (lotGameObject == null) return null;
                var lotObject = lotGameObject.GetComponent<BaseParkingLotObject>();
                lotObject.PopulateObject(data);
                return lotObject;
            }
            catch(Exception ex)
            {
                Debug.LogError($"Failed to create object [{data.lotObjectSubType}] of type [{data.lotObjectType.ToString()}], {ex.Message}");
                return null;
            }
        }
    }
}