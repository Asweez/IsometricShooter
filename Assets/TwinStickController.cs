using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TwinStickController : NetworkBehaviour{

    Animator animator;
    Rigidbody rigidbody;

    public float walkSpeed, runSpeed;
    public LayerMask floorMask, itemPickupMask;
    public GameObject heldItem;
    public GameObject cameraPrefab;
    public GameObject bulletPrefab;
    [SyncVar]
    public float health;
    public Transform heldItemPos;
    public LightsaberBlockHandler lightsaberBlockHandler;
    private List<Rigidbody> ragdollRBs;
    [SyncVar(hook ="Ragdoll")]
    public bool isRagdoll = false;

	// Use this for initialization
	void Awake () {
        health = 100f;
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        lightsaberBlockHandler = GetComponentInChildren<LightsaberBlockHandler>();
        ragdollRBs = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        ragdollRBs.Remove(rigidbody);
        hips = animator.GetBoneTransform(HumanBodyBones.Hips);
	}

    void Ragdoll(bool ragdoll)
    {
        isRagdoll = ragdoll;
        animator.enabled = !ragdoll;
        if (heldItem != null)
        {
            heldItem.transform.SetParent(ragdoll ? animator.GetBoneTransform(HumanBodyBones.RightHand) : heldItemPos);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;
            heldItem.transform.localScale = Vector3.one;
        }
        foreach(Rigidbody r in ragdollRBs)
        {
            r.isKinematic = !ragdoll;
        }
    }

    Transform hips;

	
	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer)
        {
            return;
        }

        if(Input.GetButtonDown("Pickup Item") && itemToPickup != null){
            if (heldItem != null)
            {
                ItemPickupManager.instance.SpawnNewItem(heldItem, transform.position + transform.forward * 2);
                lightsaberBlockHandler.lightsaber = null;
                Destroy(heldItem.gameObject);
            }
            heldItem = itemToPickup.GetComponent<ItemPickup>().itemToPickUp;
            heldItem.SetActive(true);
            heldItem.transform.SetParent(heldItemPos);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;
            heldItem.transform.localScale = Vector3.one;
            Destroy(itemToPickup);
            itemToPickup = null;
            if(heldItem.GetComponent<Lightsaber>() != null){
                lightsaberBlockHandler.lightsaber = heldItem.transform;
            }
        }
	}

    [Command]
    public void CmdFireBullet(Vector3 muzzlePos, Vector3 velocity)
    {
        GameObject newBullet = Instantiate(bulletPrefab, muzzlePos, Quaternion.identity);
        newBullet.GetComponent<Rigidbody>().velocity = velocity;
        newBullet.transform.LookAt(newBullet.transform.position + velocity);
        NetworkServer.Spawn(newBullet);
        Destroy(newBullet, 10f);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (isRagdoll)
        {
            transform.position = hips.position;
            hips.localPosition = Vector3.zero;
            return;
        }
        float speed = 0f;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h != 0f || v != 0f)
        {
            speed = walkSpeed;
            if (Input.GetButton("Sprint"))
            {
                speed = runSpeed;
            }
        }
        rigidbody.MovePosition(transform.position + ((new Vector3(1, 0, 1) * v + new Vector3(1, 0, -1) * h) * speed * Time.fixedDeltaTime));
        //rigidbody.angularVelocity = Vector3.zero;
        Turning();
        float angle = Mathf.Deg2Rad * transform.rotation.eulerAngles.y;
        angle -= Mathf.PI / 4f;
        angle = angle % (Mathf.PI * 2);
        animator.SetFloat("MotionX", (h * Mathf.Cos(angle)) + (v * Mathf.Sin(angle)));
        animator.SetFloat("MotionY", -(h * Mathf.Sin(angle)) + (v * Mathf.Cos(angle)));
    }

    public GameObject itemToPickup = null;

    IEnumerator Die()
    {
        yield return new WaitForSeconds(5f);
        for (float f = 0f; f <= 1; f += 0.01f)
        {
            GetComponent<SkinnedMeshRenderer>().material.SetFloat("Vector1_5F9D24A2", f);
            yield return new WaitForSeconds(0.01f);
        }
        RpcRespawn();
    }

    [ClientRpc]
    public void RpcRespawn()
    {
        Invoke("Respawn", 5f);
    }

    void Respawn()
    {
        if (isLocalPlayer)
        {
            GetComponent<SkinnedMeshRenderer>().material.SetFloat("Vector1_5F9D24A2", 0f);
            transform.position = Vector3.zero;
        }
        isRagdoll = false;
    }


    public float deathForce = 100f;

    [Server]
    public void TakeDamage(float damage, Rigidbody bone, Vector3 dir)
    {
        //Running on the server
        health -= damage;
        bone.AddForce(dir * deathForce, ForceMode.Impulse);
        if (health <= 0)
        {
            health = 100f;
            isRagdoll = true;
            RpcRespawn();
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
        if(heldItem != null && !isRagdoll){
            Transform leftHandPos = heldItem.transform.Find("LeftHandPos");
            Transform rightHandPos = heldItem.transform.Find("RightHandPos");
            if(leftHandPos){
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPos.position);
            }
            if (rightHandPos)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPos.position);
            }
        }
    }

    void Turning()
    {
        // Create a ray from the mouse cursor on screen in the direction of the camera.
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit variable to store information about what was hit by the ray.
        RaycastHit floorHit;

        // Perform the raycast and if it hits something on the floor layer...
        if (Physics.Raycast(camRay, out floorHit, 100f, floorMask))
        {
            // Create a vector from the player to the point on the floor the raycast from the mouse hit.
            Vector3 playerToMouse = floorHit.point - transform.position;

            // Ensure the vector is entirely along the floor plane.
            playerToMouse.y = 0f;

            // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

            // Set the player's rotation to this new rotation.
            rigidbody.MoveRotation(newRotation);
        }
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<SkinnedMeshRenderer>().material.SetColor("Color_16A84C3B", Color.green);
        Instantiate(cameraPrefab, transform.position, Quaternion.Euler(new Vector3(30, 45, 0))).GetComponent<CameraFollow>().target = transform ;
    }
}
