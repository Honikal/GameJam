using UnityEngine;
using System.Collections.Generic;
using CodeMonkey.Utils;
using System.IO;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;
    private Vector3 origin;
    private void Start()
    {
        mesh = new Mesh();
    
        GetComponent<MeshFilter>().mesh = mesh;
        origin = Vector3.zero;
    }



    private void LateUpdate()
    {
        
    
    float fov = 360f;
        int rayCount = 80;
        float angle = 0f;
        float angleIncrease = fov / rayCount;
        float viewDistance = 35f;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int [rayCount * 3];



        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <=rayCount; i++){
            Vector3 vertex;
            RaycastHit2D raycastHit2D= Physics2D.Raycast(origin, UtilsClass.GetVectorFromAngle(angle),viewDistance, layerMask );
            if(raycastHit2D.collider == null){
                vertex = origin + UtilsClass.GetVectorFromAngle(angle) * viewDistance;
            }else{
                vertex =raycastHit2D.point;
            }

            vertices[vertexIndex] = vertex;


            if(i > 0){ 
            triangles[triangleIndex+ 0] = 0;
            triangles[triangleIndex+ 1] = vertexIndex -1;
            triangles[triangleIndex+ 2] = vertexIndex;

            triangleIndex +=3;

            }
            vertexIndex ++;
            angle -= angleIncrease;

        }




        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

    public void SetOrigin(Vector3 origin){
        this.origin = origin; 
    }

    public void SetAimDirection() { 

    }

}
