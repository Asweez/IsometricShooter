﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberBlockHandler : MonoBehaviour {

    public Transform lightsaber;

    private void OnTriggerStay(Collider other)
    {
        if (lightsaber == null) return;
        if (other.GetComponent<BlasterBullet>() == null) return;
        lightsaber.GetComponent<Lightsaber>().length = 2;
        Ray ray = new Ray(other.transform.position, -transform.forward);
        Debug.DrawLine(transform.position, ray.GetPoint(-Vector3.Dot(transform.forward, transform.position) + Vector3.Dot(transform.forward, other.transform.position)));
        lightsaber.LookAt(ray.GetPoint(-Vector3.Dot(transform.forward, transform.position) + Vector3.Dot(transform.forward, other.transform.position)));
        lightsaber.Rotate(new Vector3(90f, 0, 0));
        //lightsaber.rotation = Quaternion.Euler(new Vector3(lightsaber.rotation.eulerAngles.x, 90f, lightsaber.rotation.eulerAngles.z));
        Debug.DrawLine(lightsaber.position, other.transform.position);

    }

    private void OnTriggerExit(Collider other)
    {
        if (lightsaber == null) return;
        lightsaber.GetComponent<Lightsaber>().length = 0;
    }
}
