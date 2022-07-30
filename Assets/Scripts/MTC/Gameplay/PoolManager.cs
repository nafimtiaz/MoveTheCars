using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MTC.Core.Classes;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace MTC.Gameplay
{
    public class PoolManager
    {
        private List<GameObject> poolObjects;
        private Transform poolParent;

        private static MTCGameConfig config => GameManager.GetConfig();

        #region Pool Creation

        public void CreatePool()
        {
            poolParent = new GameObject("ParkingLotObjectPool").transform;
            poolObjects = new List<GameObject>();

            AddObjectsInPool(config.vehicles,config.vehiclePoolCount);
            AddObjectsInPool(config.walls,config.wallPoolCount);
            AddObjectsInPool(config.obstacles,config.obstaclePoolCount);
            AddObjectsInPool(config.roads,config.roadPoolCount);
        }

        private void AddObjectsInPool(GameObject[] objects, int count)
        {
            foreach (GameObject obj in objects)
            {
                for (int i = 0; i < count; i++)
                {
                    GameObject newObj = GameObject.Instantiate(obj, Vector3.zero,Quaternion.identity,poolParent);
                    newObj.name = obj.name;
                    poolObjects.Add(newObj);
                    newObj.SetActive(false);
                }
            }
        }

        #endregion

        #region Fetch or Return

        public GameObject GetObjectFromPool(string subType)
        {
            GameObject[] objects = poolObjects.Where(obj => obj.name == subType)
                .ToArray();

            if (objects.Length > 0)
            {
                GameObject obj = objects[0];
                poolObjects.Remove(obj);
                poolObjects.TrimExcess();
                return obj;
            }

            return null;
        }
        
        public void ReturnObjectToPool(GameObject obj)
        {
            obj.transform.SetParent(poolParent);
            obj.transform.position = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.SetActive(false);
            poolObjects.Add(obj);
        }

        #endregion
    }
}