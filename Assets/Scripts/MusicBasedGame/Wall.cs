using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    public AudioAnalysis audioAnalysis;
    private Material material;
    private Color color;

    private void Awake()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        material = mesh.material;
        color = material.GetColor("_EmissionColor");
    }

    private void Update()
    {
        //material.SetColor("_EmissionColor", color * AudioAnalysis.Instance.RMSValue * 1.5f);

        transform.localScale = new Vector3(1, 7 + audioAnalysis.RMSBufferValue * 3f, 1);

    }

}
