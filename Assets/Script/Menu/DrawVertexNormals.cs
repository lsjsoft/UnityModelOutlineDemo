#if UNITY_EDITOR

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DrawVertexNormals : MonoBehaviour
{
    [Header("法线线段长度")]
    public float normalLength = 0.1f;
    [Header("顶点圆点大小")]
    public float vertexSphereRadius = 0.015f;

    public Color vertexColor = Color.cyan;
    public Color normalColor = Color.green;

    void OnDrawGizmosSelected()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
            return;

        Mesh mesh = mf.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        // 顶点数和法线数量不一致时直接退出
        if (vertices.Length != normals.Length)
            return;

        Transform t = transform;

        for (int i = 0; i < vertices.Length; i++)
        {
            // 顶点世界坐标
            Vector3 worldVert = t.TransformPoint(vertices[i]);
            // 法线转为世界方向（只旋转，不受缩放影响）
            Vector3 worldNormal = t.TransformDirection(normals[i]);

            // 画顶点小球
            // Gizmos.color = vertexColor;
            // Gizmos.DrawSphere(worldVert, vertexSphereRadius);

            // 画顶点法线射线：起点=顶点，沿着法线向外
            Gizmos.color = normalColor;
            Gizmos.DrawRay(worldVert, worldNormal * normalLength);
        }
    }
}

#endif
