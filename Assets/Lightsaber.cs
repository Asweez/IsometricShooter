using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightsaber : MonoBehaviour
{

    public int length = 1;
    public Material lightsaberMat;

    private List<Vector3> vertices;
    private List<Vector3> handleVerts;

    public MeshFilter meshFilter;
    public Transform bottomLeft, bottomRight, topLeft, topRight;
    public float swingSpeed = 1f;
    public float swingExtents = 1f;

    private void Awake()
    {
        vertices = new List<Vector3>();
        GameObject blade = new GameObject("LightsaberBlade");
        blade.AddComponent<MeshFilter>();
        meshFilter = blade.GetComponent<MeshFilter>();
        blade.AddComponent<MeshRenderer>();
        blade.GetComponent<MeshRenderer>().material = lightsaberMat;
        GetComponent<Light>().color = lightsaberMat.GetColor("Color_D009460D").gamma;
    }

    Vector3 start, end;
    int i = 0;
    float lerp = 0f;
    Transform min, max, center;
    Transform lbh;

    // Update is called once per frame
    void Update()
    {
        if(min == null){
            lbh = GetComponentInParent<TwinStickController>().lightsaberBlockHandler.transform;
            min = lbh.Find("Min");
            max = lbh.Find("Max");
            center = lbh.Find("Center");
        }
        CreateMesh();
        if(Input.GetButtonDown("Fire1")){
            lerp = 0;
            float rand = Random.value;
            start = lbh.InverseTransformPoint(transform.position);
            //start = new Vector3(Random.value <= 0.5f ? min.localPosition.x : max.localPosition.x, Random.value <= 0.5f ? min.localPosition.y : max.localPosition.y, min.localPosition.z);
            do
            {
                end = new Vector3(Random.value <= 0.5f ? min.localPosition.x : max.localPosition.x, Random.value <= 0.5f ? min.localPosition.y : max.localPosition.y, min.localPosition.z);
            } while (end.Equals(start));
                //if(i == 1){
            //    i = 2;
            //    start = min.localPosition;
            //    end = max.localPosition;
            //}else{
            //    i = 1;
            //    start = max.localPosition;
            //    end = min.localPosition;
            //}
        }
        transform.position = Vector3.Lerp(Vector3.Lerp(lbh.TransformPoint(start), center.position, lerp), Vector3.Lerp(center.position, lbh.TransformPoint(end), lerp), lerp);
        lerp += Time.deltaTime * swingSpeed;
        transform.LookAt(transform.parent);
        transform.Rotate(new Vector3(90f, 0, 180f));
    }

    private void OnDestroy()
    {
        Destroy(meshFilter.gameObject);
    }


    void CreateMesh(){
        while (vertices.Count > 8 + (length * 4))
        {
            vertices.RemoveAt(0);
        }
        Vector3 modifier = transform.forward * 0.2f;
        vertices.Add(bottomRight.position + modifier);
        vertices.Add(topRight.position + modifier);
        vertices.Add(topLeft.position + modifier);
        vertices.Add(bottomLeft.position + modifier);
        vertices.Add(bottomRight.position);
        vertices.Add(topRight.position);
        vertices.Add(topLeft.position);
        vertices.Add(bottomLeft.position);
        Mesh mesh = new Mesh();
        List<Vector3> newVerts = new List<Vector3>();
        for (int i = 0; i < vertices.Count; i++)
        {
            newVerts.Add(meshFilter.transform.InverseTransformVector(vertices[i]));
        }
        mesh.SetVertices(newVerts);
        int[] triangles = new int[(vertices.Count - 2) * 6];
        Vector2[] uvs = new Vector2[vertices.Count];
        uvs[0] = (Vector2.right);
        uvs[1] = new Vector2(1, 1);
        uvs[2] = (Vector2.up);
        uvs[3] = (Vector2.zero);
        int e = vertices.Count - 1;
        AddTri(new int[] { 1, 0, 3 }, triangles, 0);
        AddTri(new int[] { 2, 1, 3 }, triangles, 3);
        AddTri(new int[] { e - 1, e, e - 2 }, triangles, 6);
        AddTri(new int[] { e - 2, e, e - 3 }, triangles, 9);
        uvs[e - 3] = (Vector2.right);
        uvs[e - 2] = new Vector2(1, 1);
        uvs[e - 1] = (Vector2.up);
        uvs[e] = (Vector2.zero);
        int triAt = 12;
        for (int i = 0; i < vertices.Count - 4; i += 4)
        {
            uvs[i] = (Vector2.right);
            uvs[i + 1] = new Vector2(1, 1);
            uvs[i + 2] = (Vector2.up);
            uvs[i + 3] = (Vector2.zero);
            AddTri(new int[] { i + 5, i, i + 1 }, triangles, triAt);
            AddTri(new int[] { i + 5, i + 4, i }, triangles, triAt + 3);
            AddTri(new int[] { i + 7, i + 4, i }, triangles, triAt + 6);
            AddTri(new int[] { i + 7, i, i + 3 }, triangles, triAt + 9);
            AddTri(new int[] { i + 6, i + 5, i + 1 }, triangles, triAt + 12);
            AddTri(new int[] { i + 6, i + 1, i + 2 }, triangles, triAt + 15);
            AddTri(new int[] { i + 2, i + 3, i + 4 }, triangles, triAt + 18);
            AddTri(new int[] { i + 2, i + 4, i + 6 }, triangles, triAt + 21);
            triAt += 24;
        }
        mesh.uv = uvs;
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    void AddTri(int[] tri, int[] tris, int index)
    {
        tris[index] = tri[0];
        tris[index + 1] = tri[1];
        tris[index + 2] = tri[2];
    }
}
