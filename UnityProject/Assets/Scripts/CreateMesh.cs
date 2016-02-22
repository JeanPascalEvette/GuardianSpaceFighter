using UnityEngine;
using System.Collections;

//This function generates a disc shaped mesh. It is used along with the Stars Shader to generate stars
public class CreateMesh : MonoBehaviour 
{
    //Number of vertices is quite low because stars are fairly small and the low alpha on the borders means that you cannot discern the difference.
	[SerializeField] private Material Mat;
	[SerializeField] private float Size = 1.0f;
    [SerializeField] private int numPoints = 10;
    [SerializeField] private float radius = 0.2f;

    private MeshRenderer mMeshRenderer;
	private MeshFilter mMesh;

	public Material Material {
		get { return Mat; }
		set { Mat = value; }
	}
	
	private Vector3 [] GetVerts( float size )
    {
        Vector3 [] verts = new Vector3[numPoints+1]; 
        float myRadius = radius * size;


        //Generates a circle mesh
        int deg;
        verts[0] = new Vector3(0.0f, 0.0f, 0.0f);
        for (int i = 0; i < numPoints; i++)
        {
            deg = i * (360 / numPoints);
            verts[i+1] = new Vector3(Mathf.Cos(deg * Mathf.Deg2Rad) * myRadius, Mathf.Sin(deg * Mathf.Deg2Rad) * myRadius, 0.0f);
        }
		

		return verts;
	}
	
	private int [] GetTriangles()
	{
		int [] starTriangles = new int[(numPoints+1) * 3];

        int triangleCounter = 0;
        for(int i = 0; i <= numPoints; i++)
        {
            starTriangles[triangleCounter++] = 0;
            if (i < numPoints)
                starTriangles[triangleCounter++] = i + 1;
            else
                starTriangles[triangleCounter++] = 1;
            starTriangles[triangleCounter++] = i;
        }

		return starTriangles;
	}
	
	private Mesh DoCreateMesh()
	{
		Mesh m = new Mesh();
		m.name = "ScriptedMesh";
		m.vertices = GetVerts( Size ); 
		m.triangles = GetTriangles();
		m.RecalculateNormals();
		
		return m;
	}
	
	void Start() 
	{
		mMeshRenderer = gameObject.AddComponent<MeshRenderer>();
		mMesh = gameObject.AddComponent<MeshFilter>();
		mMesh.mesh = DoCreateMesh();
		mMeshRenderer.material = Mat;
        mMeshRenderer.material.SetFloat("_Radius", radius);


    }
}
