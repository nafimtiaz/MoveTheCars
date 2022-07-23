using System;
using System.Collections.Generic;

namespace MTC.Utils
{
    [Serializable]
    public class GameData
    {
        public List<LevelData> levelData;

        public GameData(List<LevelData> levelData)
        {
            this.levelData = levelData;
        }
        
        public GameData() { }
    }
}