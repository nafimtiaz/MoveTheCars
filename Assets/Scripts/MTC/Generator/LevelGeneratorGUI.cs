using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelBuilder))]
public class LevelGeneratorGUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelBuilder builder = (LevelBuilder)target;
        
        if(GUILayout.Button("Generate Level"))
        {
            builder.GenerateAll();
        }
        
        if(GUILayout.Button("Generate Walls"))
        {
            builder.GenerateWalls();
        }
        
        if(GUILayout.Button("Generate Obstacles"))
        {
            builder.GenerateObstacles();
        }
        
        if(GUILayout.Button("Generate Cars"))
        {
            builder.GenerateCars();
        }
    }
}