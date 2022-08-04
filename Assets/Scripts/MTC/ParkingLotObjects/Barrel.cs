using DG.Tweening;
using MTC.Utils;
using UnityEngine;

namespace MTC.ParkingLotObjects
{
    public class Barrel : Obstacle
    {
        private Sequence sequence;
        [SerializeField] private GameObject warningObject;

        public override void PopulateObject(ParkingLotObjectData data)
        {
            base.PopulateObject(data);
            
            if (sequence != null)
            {
                sequence.Kill();
            }
            
            HideObject(false);
        }

        private void HideObject(bool isHide)
        {
            warningObject.SetActive(!isHide);
            obstacleMeshTransform.gameObject.SetActive(!isHide);
        }

        public override void OnImpact(Vector3 hitPoint, bool isHitter)
        {
            sequence = DOTween.Sequence();
            sequence.AppendCallback(() => base.OnImpact(hitPoint, isHitter));
            sequence.AppendInterval(0.3f);
            sequence.AppendCallback(() =>
            {
                GameObject.Instantiate(GameManager.GetConfig().explosionParticle, transform.position + new Vector3(0f,2f,0f),
                    Quaternion.identity);
                GameManager.GetSoundManager().PlaySound(GameManager.GetConfig().explosionSound);
                HideObject(true);
                ExplodeNearbyObjects();
            });
            sequence.AppendInterval(1f);
            sequence.AppendCallback(() =>
            {
                GameManager.GetHomeView().OnGameOver();
            });
        }


        private void ExplodeNearbyObjects()
        {
            Collider[] cols = Physics.OverlapSphere(obstacleMeshTransform.position, 2f,
                GameManager.GetConfig().vehicleLayerMask);

            foreach (var col in cols)
            {
                Rigidbody rb = col.gameObject.AddComponent<Rigidbody>();
                rb.AddExplosionForce(500f,obstacleMeshTransform.position,10f,10f);
            }
        }
    }
}