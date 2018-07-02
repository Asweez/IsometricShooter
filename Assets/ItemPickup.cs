using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {

    public GameObject itemToPickUp;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    public Material hologramMaterial;
    public static Color defaultColor;

	// Use this for initialization
	void Awake () {
        meshFilter = GetComponentInChildren<MeshFilter>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        defaultColor = hologramMaterial.GetColor("Color_B11E0E63");
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

    public void UpdateColor(Color c)
    {
        for(int i = 0; i < meshRenderer.materials.Length; i++)
        {
            meshRenderer.materials[i].SetColor("Color_B11E0E63", c);
        }
    }

    public void ResetColor()
    {
        UpdateColor(defaultColor);
    }
}
