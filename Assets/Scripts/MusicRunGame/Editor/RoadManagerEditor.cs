using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadManager))]
public class RoadManagerEditor : Editor
{

    private SerializedProperty managerModeProp;
    private SerializedProperty difficultyProp;

    private SerializedProperty verticalSpeedProp;
    private SerializedProperty horizontalSpeedProp;

    private SerializedProperty roadDataProp;
    private SerializedProperty roadDataManifestProp;

    private SerializedProperty straighPathCheckingDistanceProp;
    private SerializedProperty zigzagPathCheckingMaxDistanceProp;
    private SerializedProperty zigzagPathCheckingMinCountProp;
    private SerializedProperty zigzagIntensityProp;

    private SerializedProperty obstacleProp;
    private SerializedProperty spawnDelayHeightProp;
    private SerializedProperty difficultyManagersProp;


    private void Awake()
    {
        managerModeProp = serializedObject.FindProperty("managerMode");
        difficultyProp = serializedObject.FindProperty("difficulty");

        verticalSpeedProp = serializedObject.FindProperty("verticalSpeed");
        horizontalSpeedProp = serializedObject.FindProperty("horizontalSpeed");

        roadDataProp = serializedObject.FindProperty("roadData");
        roadDataManifestProp = serializedObject.FindProperty("roadDataManifest");

        straighPathCheckingDistanceProp = serializedObject.FindProperty("straighCheckingDistance");
        zigzagPathCheckingMaxDistanceProp = serializedObject.FindProperty("zigzagCheckingMaxDistance");
        zigzagPathCheckingMinCountProp = serializedObject.FindProperty("zigzagCheckingMinCount");

        obstacleProp = serializedObject.FindProperty("obstacle");
        zigzagIntensityProp = serializedObject.FindProperty("zigzagIntensity");
        spawnDelayHeightProp = serializedObject.FindProperty("spawnDelayHeight");
        difficultyManagersProp = serializedObject.FindProperty("difficultyManagers");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(managerModeProp);
        //if (managerModeProp.enumValueIndex == (int)RoadManager.ManagerMode.Record)
        {
            EditorGUILayout.PropertyField(straighPathCheckingDistanceProp);
            EditorGUILayout.PropertyField(zigzagPathCheckingMaxDistanceProp);
            EditorGUILayout.PropertyField(zigzagPathCheckingMinCountProp);
        }
        //if (managerModeProp.enumValueIndex == (int)RoadManager.ManagerMode.Spawn)
        {
            EditorGUILayout.PropertyField(obstacleProp);
            EditorGUILayout.PropertyField(zigzagIntensityProp);
            EditorGUILayout.PropertyField(spawnDelayHeightProp);
        }

        EditorGUILayout.PropertyField(roadDataProp);
        EditorGUILayout.PropertyField(roadDataManifestProp);

        EditorGUILayout.PropertyField(verticalSpeedProp);
        EditorGUILayout.PropertyField(horizontalSpeedProp);


        EditorGUILayout.PropertyField(difficultyProp);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(difficultyManagersProp.GetArrayElementAtIndex(difficultyProp.enumValueIndex).FindPropertyRelative("initialPathExtent"));
        EditorGUILayout.PropertyField(difficultyManagersProp.GetArrayElementAtIndex(difficultyProp.enumValueIndex).FindPropertyRelative("minPathExtent"));
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }

}
