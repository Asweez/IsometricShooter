using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {

    public GameObject itemToPickUp;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    public Material hologramMaterial;

	// Use this for initialization
	void Awake () {
        meshFilter = GetComponentInChildren<MeshFilter>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
	}

    private void Start()
    {
        if (itemToPickUp != null)
        {
            UpdateMesh();
        }
    }

    public void ChangePickupItem(GameObject newPickup){
        itemToPickUp = newPickup;
        UpdateMesh();
    }

    void UpdateMesh(){
        meshFilter.mesh = itemToPickUp.GetComponent<MeshFilter>().sharedMesh;
        Material[] materials = new Material[meshFilter.sharedMesh.subMeshCount];
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = hologramMaterial;
        }
        meshRenderer.sharedMaterials = materials;
    }
}
