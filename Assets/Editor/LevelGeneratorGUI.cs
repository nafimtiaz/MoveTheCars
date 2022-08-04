using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelBuilder))]
public class LevelGeneratorGUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelBuilder builder = (LevelBuilder)target;
        
        if(GUILayout.Button("Add Object [Manual]"))
        {
            builder.AddObjectManually();
        }
        
        if(GUILayout.Button("Generate All"))
        {
            builder.GenerateAll();
        }
        
        if(GUILayout.Button("Save Level"))
        {
            builder.SaveLevel();
        }
        
        if(GUILayout.Button("Clear All"))
        {
            builder.ClearAll();
        }
        
        if(GUILayout.Button("1. Generate Walls"))
        {
            builder.GenerateWalls();
        }
        
        if(GUILayout.Button("2. Generate Obstacles"))
        {
            builder.GenerateObstacles();
        }
        
        if(GUILayout.Button("3. Generate Cars"))
        {
            builder.GenerateVehicles();
        }
    }
}