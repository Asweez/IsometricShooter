using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BlasterRifle : MonoBehaviour {

    public GameObject impactPS, playerImpactPS;
    public float bulletSpeed = 1f;
    public float fireRate = 0.1f;
    public float bulletDamage = 15f;
    public float horizontalSpread = 1f;
    public float verticalSpread = 1f;
    private float fireTime = 0;
    private Transform muzzle;
    private TwinStickController twinStickController;

    private void Awake()
    {
        muzzle = transform.Find("Muzzle");
    }

    void OnEnable()
    {
        twinStickController = GetComponentInParent<TwinStickController>();
    }

    // Update is called once per frame
    void Update () {


        if(Input.GetButton("Fire1") && Time.time - fireTime >= fireRate && twinStickController.health > 0 && twinStickController.isLocalPlayer){
            Fire();
        }
	}

    void Fire()
    {
        Vector3 velocity = muzzle.forward;
        velocity = Quaternion.AngleAxis(Random.Range(-horizontalSpread, horizontalSpread), Vector3.up) * velocity;
        velocity = Quaternion.AngleAxis(Random.Range(-verticalSpread, verticalSpread), muzzle.right) * velocity;
        twinStickController.CmdFireBullet(muzzle.position, velocity * bulletSpeed);
        fireTime = Time.time;
    }
}
