using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;
    public Vector3 offset;



	// Update is called once per frame
	void LateUpdate () {

        MoveCamera();
    }

    private void MoveCamera()
    {
        Vector3 camPos = new Vector3(transform.position.x, target.position.y,transform.position.z) + offset;
        transform.position = camPos;
    }
}
