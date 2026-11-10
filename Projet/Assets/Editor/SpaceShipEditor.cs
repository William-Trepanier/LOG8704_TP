using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpaceShip))]
public class SpaceShipEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        if (GUILayout.Button("Increment Rapidity"))
            ((SpaceShip)target).IncrementRapidity(false);
        if (GUILayout.Button("Decrement Rapidity"))
            ((SpaceShip)target).DecrementRapidity(false);
    }
}
