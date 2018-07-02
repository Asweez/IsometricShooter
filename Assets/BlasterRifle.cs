using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterRifle : MonoBehaviour {

    public GameObject blasterBullet;
    public GameObject impactPS, playerImpactPS;
    public float bulletSpeed = 1f;
    public float fireRate = 0.1f;
    public float bulletDamage = 15f;
    public float horizontalSpread = 1f;
    public float verticalSpread = 1f;
    private float fireTime = 0;
    private Transform muzzle;

    private void Awake()
    {
        muzzle = transform.Find("Muzzle");
    }

    // Update is called once per frame
    void Update () {
        if(Input.GetButton("Fire1") && Time.time - fireTime >= fireRate && GetComponentInParent<TwinStickController>().Health > 0){
            GameObject newBullet = Instantiate(blasterBullet, muzzle.position, Quaternion.identity);
            Vector3 velocity = muzzle.forward;
            velocity = Quaternion.AngleAxis(Random.Range(-horizontalSpread, horizontalSpread), Vector3.up) * velocity;
            velocity = Quaternion.AngleAxis(Random.Range(-verticalSpread, verticalSpread), muzzle.right) * velocity;
            newBullet.GetComponent<Rigidbody>().velocity = velocity * bulletSpeed;
            newBullet.transform.LookAt(newBullet.transform.position + velocity);
            newBullet.GetComponent<BlasterBullet>().blasterRifle = this;
            fireTime = Time.time;
        }
	}

    public void BulletHit(Vector3 point, TwinStickController player, Collider c, Vector3 dir){
        Destroy(Instantiate(impactPS, point, Quaternion.identity), 1.5f);
        if (player)
        {
            Destroy(Instantiate(playerImpactPS, point, Quaternion.identity), 1.5f);
            player.TakeDamage(bulletDamage, c.GetComponent<Rigidbody>(), dir.normalized);
        }
    }
}
