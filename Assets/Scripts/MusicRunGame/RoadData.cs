using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "RoadData", menuName = "RoadData",order = 0)]
public class RoadData : ScriptableObject {

    public List<Vector2> positions;


    public void Clear()
    {
        positions.Clear();
    }

#if UNITY_EDITOR
    [MenuItem("CONTEXT/ScriptableObject/Save Data To Manifest")]
    public static void SaveDataManifest(MenuCommand command)
    {
        RoadDataManifest roadDataManifest = FindObjectOfType<RoadDataManifest>();
        if(roadDataManifest==null)
        {
            Debug.LogWarning("Cannot find a RoadDataManifest component, a new one has been created");
            GameObject newObject = new GameObject("RoadDataManifest");
            roadDataManifest = newObject.AddComponent<RoadDataManifest>();
        }

        roadDataManifest.positions = ((RoadData)(command.context)).positions.ToArray();
    }
#endif  

    public void SaveDataManifest()
    {
        RoadDataManifest roadDataManifest = FindObjectOfType<RoadDataManifest>();
        if (roadDataManifest == null)
        {
            Debug.LogWarning("Cannot find a RoadDataManifest component, a new one has been created");
            GameObject newObject = new GameObject("RoadDataManifest");
            roadDataManifest = newObject.AddComponent<RoadDataManifest>();
        }
        roadDataManifest.positions = positions.ToArray();
    }
 

}
