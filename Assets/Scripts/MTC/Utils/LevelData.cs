using System;
using System.Collections.Generic;

namespace MTC.Utils
{
    [Serializable]
    public class LevelData
    {
        public int levelIndex;

        public List<ParkingLotObjectData> lotObjects;
    }
}