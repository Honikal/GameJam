using UnityEngine;

public class LayerByMeshPosition : MonoBehaviour
{
    public MeshFilter targetMeshFilter; // Assign your mesh in inspector
    public string visibleLayerName = "Visible";
    public string defaultLayerName = "Default";
    public float checkInterval = 0.1f; // How often to check (seconds)

    private Mesh _mesh;
    private int _visibleLayer;
    private int _defaultLayer;
    private float _timer;

    void Start()
    {
        if (targetMeshFilter == null)
        {
            Debug.LogError("No mesh filter assigned!");
            enabled = false;
            return;
        }

        _mesh = targetMeshFilter.mesh;
        _visibleLayer = LayerMask.NameToLayer(visibleLayerName);
        _defaultLayer = LayerMask.NameToLayer(defaultLayerName);

        // Verify layers exist
        if (_visibleLayer == -1 || _defaultLayer == -1)
        {
            Debug.LogError("One or more layers don't exist!");
            enabled = false;
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= checkInterval)
        {
            _timer = 0;
            CheckPositionForAllObjects();
        }
    }

    void CheckPositionForAllObjects()
    {
        // Get all objects that should be checked (modify as needed)
        GameObject[] objectsToCheck = GameObject.FindGameObjectsWithTag("DynamicObject"); 

        foreach (GameObject obj in objectsToCheck)
        {
            bool isInside = IsPointInMesh(obj.transform.position);
            obj.layer = isInside ? _visibleLayer : _defaultLayer;
        }
    }

    bool IsPointInMesh(Vector3 point)
    {
        // Convert point to mesh's local space
        Vector3 localPoint = targetMeshFilter.transform.InverseTransformPoint(point);

        // Simple AABB check first for performance
        if (!_mesh.bounds.Contains(localPoint)) 
            return false;

        // Exact mesh check (more expensive)
        return IsPointInMeshExact(_mesh, localPoint);
    }

    bool IsPointInMeshExact(Mesh mesh, Vector3 point)
    {
        // Raycast in +X direction and count intersections
        int intersections = 0;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v1 = vertices[triangles[i]];
            Vector3 v2 = vertices[triangles[i+1]];
            Vector3 v3 = vertices[triangles[i+2]];

            if (RayIntersectsTriangle(point, Vector3.right, v1, v2, v3))
                intersections++;
        }

        // Odd number of intersections = inside
        return intersections % 2 == 1;
    }

    // Ray-Triangle intersection test
    bool RayIntersectsTriangle(Vector3 origin, Vector3 dir, 
                              Vector3 v0, Vector3 v1, Vector3 v2)
    {
        // Möller–Trumbore intersection algorithm
        Vector3 e1 = v1 - v0;
        Vector3 e2 = v2 - v0;
        Vector3 h = Vector3.Cross(dir, e2);
        float a = Vector3.Dot(e1, h);

        if (a > -Mathf.Epsilon && a < Mathf.Epsilon)
            return false;

        float f = 1.0f / a;
        Vector3 s = origin - v0;
        float u = f * Vector3.Dot(s, h);

        if (u < 0.0 || u > 1.0)
            return false;

        Vector3 q = Vector3.Cross(s, e1);
        float v = f * Vector3.Dot(dir, q);

        if (v < 0.0 || u + v > 1.0)
            return false;

        float t = f * Vector3.Dot(e2, q);
        return t > Mathf.Epsilon;
    }
}