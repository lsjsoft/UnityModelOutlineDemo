using UnityEngine;

public class AutoBakeNormal : MonoBehaviour
{
    void Start()
    {
        var meshFilter = this.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            var srcMesh = meshFilter.sharedMesh;
            if (srcMesh != null )
            {
                srcMesh.uv7 = AquaSys.SmoothNormals.AquaSmoothNormals.ComputeSmoothedNormals(srcMesh);
            }
        }
    }
}