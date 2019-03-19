using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadDataManifest : MonoBehaviour {

    public Vector2[] positions;



#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawSphere(positions[i], 0.5f);
        }

    }

#endif

}
