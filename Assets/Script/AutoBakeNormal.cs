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
                // 8面体压缩法线(2D)
                var uv6_v2 = AquaSys.SmoothNormals.AquaSmoothNormals.ComputeSmoothedNormals(srcMesh);
                srcMesh.SetUVs(6, uv6_v2);

                // 烘焙法线(3D)
                var uv7_v3 = AquaSys.SmoothNormals.AquaSmoothNormals.ComputeSmoothedNormals3(srcMesh);
                srcMesh.SetUVs(7, uv7_v3);
            }
        }
    }
}