using MTC.Utils;
using UnityEngine;

namespace MTC.ParkingLotObjects
{
    public class PoliceCar : Vehicle
    {
        [SerializeField] private GameObject warningSign;
        private bool isAlert;

        public override void PopulateObject(ParkingLotObjectData data)
        {
            base.PopulateObject(data);
            SetAlert(false);
        }

        private void SetAlert(bool yes)
        {
            isAlert = yes;
            warningSign.SetActive(yes);

            if (yes)
            {
                GameManager.GetSoundManager().PlaySound(GameManager.GetConfig().heySound);
            }
        }


        public override void OnImpact(Vector3 hitPoint, bool isHitter)
        {
            base.OnImpact(hitPoint, isHitter);

            if (!isHitter)
            {
                if (!isAlert)
                {
                    SetAlert(true);
                }
                else
                {
                    SetAlert(false);
                    GameManager.GetSoundManager().PlaySound(GameManager.GetConfig().sirenSound);
                    GameManager.GetHomeView().OnGameOver();
                }
            }
        }

        public override void OnSuccess()
        {
            base.OnSuccess();
            SetAlert(false);
        }
    }
}
