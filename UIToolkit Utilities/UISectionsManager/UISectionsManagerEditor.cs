#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UISectionsManager))]
public class UISectionsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var manager = (UISectionsManager)target;

        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "Sections");

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Sections", EditorStyles.boldLabel);

        if (manager.Sections == null || manager.Sections.Count == 0)
        {
            EditorGUILayout.HelpBox("No sections found. Make sure your UI elements have the correct class and unique names.", MessageType.Info);
        }
        else
        {
            EditorGUI.BeginDisabledGroup(true);
            foreach (var section in manager.Sections)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.TextField(section.sectionName);

                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(6);

            foreach (var section in manager.Sections)
            {
                EditorGUILayout.BeginHorizontal();

                section.visible = EditorGUILayout.Toggle(section.visible, GUILayout.Width(20));
                EditorGUILayout.LabelField(section.sectionName);

                EditorGUILayout.EndHorizontal();
            }
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(manager);
    }
}
#endif