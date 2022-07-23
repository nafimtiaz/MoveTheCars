using UnityEngine;
using System;
using System.IO;

namespace MTC.Utils
{
    public static class DataManager
    {
        #region Save/Load JSON Data
        
        public static void SaveDataToJson<T>(T dataToSave)
        {
            #if UNITY_EDITOR
            
            try
            {
                using (FileStream fs = new FileStream(GetJsonFilePath<T>(), FileMode.Create)){
                    using (StreamWriter writer = new StreamWriter(fs)){
                        writer.Write(JsonUtility.ToJson(dataToSave));
                    }
                }
                
                Debug.Log("Level saved successfully!");
            }
            catch (Exception e)
            {
                Debug.Log("Level saving failed!");
            }
            
            #endif
        }

        public static T LoadDataFromJson<T>() where T : class,new()
        {
            T data;
            string dataString = "";
            bool isFileAvailable = false;

            try
            {
                TextAsset textAsset = Resources.Load($"Data/{typeof(T).Name}") as TextAsset;

                if (textAsset != null)
                {
                    dataString = textAsset.ToString();
                    isFileAvailable = true;
                }
            }
            catch
            {
                // ignored
            }

            if (isFileAvailable)
            {
                try
                {
                    data = JsonUtility.FromJson<T>(dataString);
                }
                catch (Exception e)
                {
                    data = new T();
                }
            }
            else
            {
                data = new T();
            }

            return data;
        }
        
        private static string GetJsonFilePath<T>()
        {
            return $"Assets/Resources/Data/{typeof(T).Name}.json";
        }

        #endregion
    }
}