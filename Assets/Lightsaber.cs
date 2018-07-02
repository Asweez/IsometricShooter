using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightsaber : MonoBehaviour
{

    public float length = 1;
    public float angleThreshold = 20f;
    public bool blocking = false;
    public float blockingColliderIncreaseMultiplier = 2f;
    public Material lightsaberMat;

    private List<Vector3> vertices;
    private List<Vector3> handleVerts;

    public MeshFilter meshFilter;
    public Transform bottomLeft, bottomRight, topLeft, topRight;
    public float swingSpeed = 1f;

    private void Awake()
    {
        vertices = new List<Vector3>();
        GameObject blade = new GameObject("LightsaberBlade");
        blade.transform.SetParent(transform);
        blade.transform.localPosition = Vector3.zero;
        blade.transform.localScale = Vector3.one;
        blade.transform.localRotation = Quaternion.identity;
        blade.AddComponent<MeshFilter>();
        meshFilter = blade.GetComponent<MeshFilter>();
        blade.AddComponent<MeshRenderer>();
        blade.GetComponent<MeshRenderer>().material = lightsaberMat;
        blade.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        GetComponent<Light>().color = lightsaberMat.GetColor("Color_D009460D").gamma;
        prevLocalRot = transform.localRotation;
        CreateMesh();
        StartCoroutine(Extend(true));
    }

    private void OnEnable()
    {
        defaultLocalPos = transform.localPosition;
        defaultLocalRot = transform.localRotation;
    }

    Vector3 defaultLocalPos;
    Quaternion defaultLocalRot;

    Vector3 start, end;
    float lerp = 0f;
    float angle = 45f;
    Transform min, max, center;
    Transform lbh;
    Vector3 dir;
    // Update is called once per frame
    void Update()
    {
        if (GetComponentInParent<TwinStickController>().Health <= 0) return;
        blocking = Input.GetButton("Fire2");
        if(Input.GetButtonDown("Fire2")){
            GetComponent<BoxCollider>().size = new Vector3(GetComponent<BoxCollider>().size.x * blockingColliderIncreaseMultiplier, GetComponent<BoxCollider>().size.y, GetComponent<BoxCollider>().size.z * blockingColliderIncreaseMultiplier);
        }
        if (Input.GetButtonUp("Fire2"))
        {
            GetComponent<BoxCollider>().size = new Vector3(GetComponent<BoxCollider>().size.x / blockingColliderIncreaseMultiplier, GetComponent<BoxCollider>().size.y, GetComponent<BoxCollider>().size.z / blockingColliderIncreaseMultiplier);
        }
        if (blocking)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, defaultLocalPos, Time.deltaTime * 4);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, defaultLocalRot, Time.deltaTime * 4);
        }
        else
        {
            if (min == null)
            {
                lbh = GetComponentInParent<TwinStickController>().lightsaberBlockHandler.transform;
                min = lbh.Find("Min");
                max = lbh.Find("Max");
                center = lbh.Find("Center");
            }
            if (Input.GetButtonDown("Fire1") && lerp >= 0.8f)
            {
                lerp = 0;
                float rand = Random.value;
                start = lbh.InverseTransformPoint(transform.position);
                do
                {
                    end = new Vector3(Mathf.Lerp(min.localPosition.x, max.localPosition.x, (Mathf.Cos(angle * Mathf.Deg2Rad) / 2f) + 0.5f), Mathf.Lerp(min.localPosition.y, max.localPosition.y, (Mathf.Sin(angle * Mathf.Deg2Rad) / 2f) + 0.5f), min.localPosition.z);
                } while (end.Equals(start));
                angle = (angle + 150) % 360;
            }
            if (lerp <= 2)
            {
                Vector3 prevPos = transform.position;
                transform.position = Vector3.Slerp(Vector3.Lerp(lbh.TransformPoint(start), center.position, lerp), Vector3.Lerp(center.position, lbh.TransformPoint(end), lerp), lerp);
                dir = transform.position - prevPos;
                lerp += Time.deltaTime * swingSpeed;
                transform.LookAt(transform.parent);
                transform.Rotate(new Vector3(90f, 0, 180f));
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, defaultLocalPos, Time.deltaTime * 4);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, defaultLocalRot, Time.deltaTime * 4);
                //transform.Rotate(transform.parent.forward, Time.deltaTime * rotSpeed);
            }
        }
    }

    bool shouldDoDamage = true;

    void OnTriggerEnter(Collider other)
    {
        if(shouldDoDamage && !other.isTrigger && other.GetComponentInParent<TwinStickController>() != null && other.GetComponentInParent<TwinStickController>() != GetComponentInParent<TwinStickController>())
        {
            other.GetComponentInParent<TwinStickController>().TakeDamage(50f, other.GetComponent<Rigidbody>(), dir);
            shouldDoDamage = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger && other.GetComponentInParent<TwinStickController>() != null && other.GetComponentInParent<TwinStickController>() != GetComponentInParent<TwinStickController>())
        {
            shouldDoDamage = true;
        }
    }

    private void LateUpdate()
    {
        CreateMesh();
    }

    private void OnDestroy()
    {
        if (meshFilter != null)
        {
            Destroy(meshFilter.gameObject);
        }
    }

    public IEnumerator Extend(bool extend)
    {
        GetComponent<Light>().enabled = extend;
        for(float i = extend ? 0 : 1; extend ? i <= 1 : i >= 0; i += extend ? 0.02f : -0.02f)
        {
            meshFilter.GetComponent<MeshRenderer>().material.SetFloat("Vector1_67099CF8", i);
            yield return new WaitForSeconds(0.01f);
        }
    }

    Quaternion prevLocalRot;

    void CreateMesh(){
        Quaternion delta = Quaternion.Inverse(transform.localRotation) * prevLocalRot;
        float angle;
        Vector3 axis;
        delta.ToAngleAxis(out angle, out axis);
        Vector3 modifier = Vector3.forward * bottomRight.localPosition.z * -2;
        vertices.Clear();
        vertices.Add(bottomRight.localPosition + modifier);
        vertices.Add(topRight.localPosition + modifier);
        vertices.Add(topLeft.localPosition + modifier);
        vertices.Add(bottomLeft.localPosition + modifier);
        vertices.Add(bottomRight.localPosition);
        vertices.Add(topRight.localPosition);
        vertices.Add(topLeft.localPosition);
        vertices.Add(bottomLeft.localPosition);

        for (float f = 0.25f; f <= length; f += 0.25f)
        {
            delta = Quaternion.AngleAxis(angle * f, axis);
            if (angle * f > angleThreshold) break;
            vertices.Add(delta * bottomRight.localPosition);
            vertices.Add(delta * topRight.localPosition);
            vertices.Add(delta * topLeft.localPosition);
            vertices.Add(delta * bottomLeft.localPosition);
        }

        //for (int i = 1; i < vertices.Count; i++){
        //    Debug.DrawLine(transform.TransformPoint(vertices[i]), transform.TransformPoint(vertices[i - 1]), Color.black);
        //}

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
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
        prevLocalRot = transform.localRotation;
    }

    void UpdateMesh(){
        Quaternion delta = transform.localRotation * Quaternion.Inverse(prevLocalRot);
        //float angle;
        //Vector3 axis;
        //delta.ToAngleAxis(out angle, out axis);
        //delta = Quaternion.AngleAxis(angle * length, axis);
        vertices[8] = delta * bottomRight.localPosition;
        vertices[9] = delta * topRight.localPosition;
        vertices[10] = delta * topLeft.localPosition;
        vertices[11] = delta * bottomLeft.localPosition;
        Debug.DrawLine(bottomRight.position, transform.TransformPoint(delta * topLeft.localPosition), Color.black);

        meshFilter.mesh.SetVertices(vertices);
        meshFilter.mesh.RecalculateBounds();

        prevLocalRot = transform.localRotation;

    }

    void AddTri(int[] tri, int[] tris, int index)
    {
        tris[index] = tri[0];
        tris[index + 1] = tri[1];
        tris[index + 2] = tri[2];
    }
}
