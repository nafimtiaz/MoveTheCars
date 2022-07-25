using UnityEngine;

namespace MTC.Gameplay
{
    public class TouchManager : MonoBehaviour
    {
        private bool isTouchRecorded;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask vehicleLayer;
        private Vector3 touchStartPos = Vector3.zero;
        private Vector3 touchEndPos = Vector3.zero;
        private Transform currentSelectedVehicle;

        private readonly Vector3[] globalDirs = 
        {
            Vector3.forward, 
            Vector3.left, 
            Vector3.back, 
            Vector3.right
        };

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch t = Input.touches[0];

                switch (t.phase)
                {
                    case TouchPhase.Began:
                        isTouchRecorded = false;
                        SelectVehicle(Camera.main.ScreenPointToRay(t.position));
                        break;
                    case TouchPhase.Moved:
                        if (!isTouchRecorded)
                        {
                            GetVehicleDir(Camera.main.ScreenPointToRay(t.position));
                            isTouchRecorded = true;
                        }
                        break;
                    case TouchPhase.Ended:
                        isTouchRecorded = false;
                        break;
                }
            }
        }

        private void SelectVehicle(Ray ray)
        {
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, vehicleLayer))
            {
                currentSelectedVehicle = hit.transform;
            }
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                touchStartPos = hit.point;
            }
        }
        
        private void GetVehicleDir(Ray ray)
        {
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                touchEndPos = hit.point;
                
                if (currentSelectedVehicle != null)
                {
                    Vector3 dir = (touchEndPos - touchStartPos).normalized;
                    Vector3 targetDir = GetGlobalDirection(dir);
                    currentSelectedVehicle.GetComponent<Vehicle>().CheckAndMove(targetDir);
                }
            }
        }

        private Vector3 GetGlobalDirection(Vector3 absDir)
        {
            float tempAngDist = Mathf.Infinity;
            Vector3 dir = Vector3.zero;

            for (int i = 0; i < globalDirs.Length; i++)
            {
                float ad = Mathf.Acos(Vector3.Dot(absDir, globalDirs[i]));
                if (ad < tempAngDist)
                {
                    dir = globalDirs[i];
                    tempAngDist = ad;
                }
            }

            return dir;
        }
    }
}