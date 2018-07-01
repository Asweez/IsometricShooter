using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform target;
    public float speed = 1f;
    public Vector3 offset;

    private void Start()
    {
        offset = target.position - transform.position;
    }

    // Update is called once per frame
    void Update () {
        transform.position = Vector3.Lerp(transform.position, target.position - offset, Time.deltaTime * speed);
	}
}
