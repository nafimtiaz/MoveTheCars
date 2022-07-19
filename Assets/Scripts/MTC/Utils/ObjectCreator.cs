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
        public static BaseParkingLotObject CreateAndPlaceObject(ParkingLotObjectData data)
        {
            try
            {
                var assetPath = $"Objects/{data.lotObjectType.ToString()}/{data.lotObjectSubType}";
                var lotGameObject = Object.Instantiate(Resources.Load(assetPath)) as GameObject;

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