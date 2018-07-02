using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupManager : MonoBehaviour {

    public static ItemPickupManager instance;

    public GameObject itemPickupPrefab;
    public float itemSpawnVerticalSpeed, itemSpawnHorizontalSpeed;
    //public GameObject[] gunsToRandomlySpawn;

    private void Awake()
    {
        if(instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }
    }

    public void SpawnNewItem(GameObject item, Vector3 position){
        GameObject newItemPickup = Instantiate(itemPickupPrefab, transform);
        newItemPickup.transform.position = position;
        GameObject itemToPickup = Instantiate(item);
        itemToPickup.SetActive(false);
        newItemPickup.GetComponent<ItemPickup>().ChangePickupItem(itemToPickup);
        newItemPickup.GetComponent<Rigidbody>().AddForce(new Vector3((Random.value - 0.5f) * itemSpawnHorizontalSpeed, itemSpawnVerticalSpeed, (Random.value - 0.5f) * itemSpawnHorizontalSpeed), ForceMode.Impulse);
    }
}
