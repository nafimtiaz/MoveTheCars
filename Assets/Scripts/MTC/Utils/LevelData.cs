using System;
using System.Collections.Generic;

namespace MTC.Utils
{
    [Serializable]
    public class LevelData
    {
        public int levelIndex;
        
        public int length;

        public int width;
        
        public string levelMessage;

        public List<ParkingLotObjectData> lotObjects;

        public LevelData(
            int levelIndex,
            int length,
            int width,
            string levelMessage, 
            List<ParkingLotObjectData> lotObjects)
        {
            this.levelIndex = levelIndex;
            this.length = length;
            this.width = width;
            this.lotObjects = lotObjects;
            this.levelMessage = levelMessage;
        }
    }
}