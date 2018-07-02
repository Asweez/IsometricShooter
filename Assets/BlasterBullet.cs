using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterBullet : MonoBehaviour {

    public BlasterRifle blasterRifle;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.GetComponent<Lightsaber>() != null)
        {
            
            GetComponent<Rigidbody>().velocity = Vector3.Reflect(GetComponent<Rigidbody>().velocity, other.transform.parent.forward);
            transform.LookAt(transform.position + GetComponent<Rigidbody>().velocity);
        }
        else
        {
            
            if (!other.isTrigger)
            {
                if (blasterRifle != null)
                {
                    blasterRifle.BulletHit(transform.position - transform.forward, other.GetComponentInParent<TwinStickController>(), other, transform.forward);
                }
                Destroy(gameObject);
            }
        }
    }
}
