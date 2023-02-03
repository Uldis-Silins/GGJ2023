using UnityEditor;

[CustomEditor(typeof(InspectorElement))]

public class InspectorCustomizer : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Levels"), true);
        serializedObject.ApplyModifiedProperties();
    }
}