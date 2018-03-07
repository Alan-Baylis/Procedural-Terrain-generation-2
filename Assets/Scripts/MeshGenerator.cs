using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshCollider))] //container that passes the mesh's information to the renderer

[ExecuteInEditMode] //we can edit it in unity

public abstract class MeshGenerator : MonoBehaviour
{
    [SerializeField] protected Material material; //to make it display in the inspector

    protected List<Vector3> vertices;
    protected List<int> triangles;

    protected List<Vector3> normals;
    protected List<Vector4> tangents;
    protected List<Vector2> UVs;
    protected List<Color32> vertexColors;


    protected MeshFilter meshFilter;
    protected MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    protected Mesh mesh; //responsible for storing all the mesh information like vertices, normals, tangents, triangles.


    protected int numVertices;
    protected int numTriangles;


    private void Update()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        meshRenderer.material = material; //individual material, creates a separate copy each time it's used

        //INITIALIZATION
        InitMesh();


        SetMeshNumber();

        CreateMesh(); //creating the mesh

    }

    protected abstract void SetMeshNumber(); //because number of vertices and triangles for each mesh will be different

    private bool ValidateMesh()
    {
      
            //string containing errors
            string errorStr = "";
            //check for correct number of triangles and vertices
            errorStr += vertices.Count == numVertices ? "" : "Should be " + numVertices + "vertices, but there are " + vertices.Count + ".";//if correct, leave as it is, or else print how many there should be and how many there are 
            errorStr += triangles.Count == numTriangles ? "" : "Should be " + numTriangles + "vertices, but there are " + triangles.Count + ".";

        //there should be same number of normals as vertices; we need to check every other parameter individually
        //not manually calculating normals yet
        bool isValid = string.IsNullOrEmpty(errorStr);
        errorStr += (normals.Count == numTriangles || normals.Count == 0) ? "" : "Should be " + numVertices + "vertices, but there are " + normals.Count + ".";
            errorStr += (tangents.Count == numTriangles || tangents.Count == 0) ? "" : "Should be " + numVertices + "vertices, but there are " + tangents.Count + ".";
            errorStr += (UVs.Count == numTriangles || UVs.Count == 0) ? "" : "Should be " + numVertices + "vertices, but there are " + UVs.Count + ".";
            errorStr += (vertexColors.Count == numTriangles || vertexColors.Count == 0) ? "" : "Should be " + numVertices + "vertices, but there are " + vertexColors.Count + ".";

        
        if (!isValid)
        {
            //Debug.LogError("Not drawing mesh. ");

        }
        return isValid;
    }

    private void InitMesh()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();

        normals = new List<Vector3>();
        tangents = new List<Vector4>();
        UVs = new List<Vector2>();
        vertexColors = new List<Color32>();
    }

    private void CreateMesh() //method that creates the mesh itself
    {
        mesh = new Mesh(); //new mesh object to assign to the variable

        SetVertices();
        SetTriangles();
        SetUVs();
        SetVertexColors();
   



        if (ValidateMesh())
        {
            //so we only assign and create the mesh if it's valid
            //should be always done in this order according to unity; vertices then triangles
            mesh.SetVertices(vertices); //setting vertices and passing the vertices list
            mesh.SetTriangles(triangles, 0); //setting triangles fro each sub mesh for which it takes an int, which decides which material is to be used. 0 becuz only 1 material

            //normals calculated automatically
            if (normals.Count == 0)
            {
                mesh.RecalculateNormals();
                normals.AddRange(mesh.normals);
            }

            mesh.SetNormals(normals);
            mesh.SetTangents(tangents);
            mesh.SetUVs(0, UVs);
            mesh.SetColors(vertexColors);

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh; //be the mesh that was just created, not shared mesh because we don't want to modify the other meshes

            

        }

        

    }

    protected abstract void SetVertices();
    protected abstract void SetTriangles();


    protected abstract void SetUVs();

    protected abstract void SetVertexColors();
   


}
