using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(RotationEffector))]
public class RotationEffectorEditor : Editor
{
    private readonly string[] _angleOptions = { "X", "Y", "Z" };
    private SerializedProperty _observedAngle;

    public void OnEnable()
    {
        _observedAngle = serializedObject.FindProperty("observedAngle");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        DrawPropertiesExcluding(serializedObject, "onRotationChanged", "observedAngle");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Observed Angle");
        _observedAngle.intValue = GUILayout.SelectionGrid(_observedAngle.intValue, _angleOptions, 3, EditorStyles.radioButton);
        GUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onRotationChanged"));
        
        serializedObject.ApplyModifiedProperties();
    }
}