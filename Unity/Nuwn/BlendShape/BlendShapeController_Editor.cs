using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlendShapeController))]
public class BlendShapeController_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BlendShapeController Script = (BlendShapeController)target;
        if (GUILayout.Button("Reset/Set List With Avalible BlendShapes"))
        {
            Script.PopulateListWithBlendShapes();
        }
    }
}
