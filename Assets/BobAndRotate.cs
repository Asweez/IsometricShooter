using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobAndRotate : MonoBehaviour {

    public float bobSpeed = 1f;
    public float bobAmount = 1f;
    public float rotateSpeed = 1f;
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.up * Mathf.Cos(Time.time * bobSpeed) * bobAmount * Time.deltaTime);
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
	}
}
