using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BlasterBullet : NetworkBehaviour {


    public GameObject particleSystem;

    [ServerCallback]
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
                if (other.GetComponent<TwinStickController>() != null)
                {
                    other.GetComponent<TwinStickController>().TakeDamage(15f, other.GetComponent<Rigidbody>(), transform.forward);
                }
                Destroy(gameObject);
                SpawnParticles();
            }
        }
    }

    public void SpawnParticles()
    {
        GameObject hitPS = Instantiate(particleSystem, transform.position, Quaternion.identity);
        NetworkServer.Spawn(hitPS);
        Destroy(hitPS, 3f);
    }
}
