using System.Collections.Generic;
using UnityEngine;

namespace AquaSys.SmoothNormals
{
public class AquaSmoothNormals
{
    public static Vector2[] ComputeSmoothedNormals(Mesh mesh)
    {
        Vector3[] verts = mesh.vertices;
        Vector3[] nors = mesh.normals;
        Vector4[] tans = mesh.tangents;

        int vertexCount = verts.Length;
        if (tans.Length == 0)
        {
            return null;
        }

        Dictionary<Vector3, Vector3> smoothedNormalsMap = new(vertexCount);
        for (int i = 0; i < vertexCount; i++)
        {
            if (smoothedNormalsMap.TryGetValue(verts[i], out Vector3 normal))
            {
                smoothedNormalsMap[verts[i]] = normal + nors[i];
            }
            else
            {
                smoothedNormalsMap.Add(verts[i], nors[i]);
            }
        }

        Vector2[] bakedSmoothedNormals = new Vector2[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 smoothedNormal = smoothedNormalsMap[verts[i]];
            Vector3 normalOS = nors[i].normalized;
            Vector3 tangentOS = ((Vector3)tans[i]).normalized;
            Vector3 bitangentOS = (Vector3.Cross(normalOS, tangentOS) * tans[i].w).normalized;
            Matrix4x4 tbn = new Matrix4x4(tangentOS, bitangentOS, normalOS, Vector3.zero).transpose;
            Vector3 tangentSpaceNormal = tbn.MultiplyVector(smoothedNormal).normalized;
            bakedSmoothedNormals[i] = OctahedronNormal(tangentSpaceNormal);
        }

        return bakedSmoothedNormals;
    }

    private static Vector2 OctahedronNormal(Vector3 ResultNormal)
    {
        Vector3 absVec = new Vector3(Mathf.Abs(ResultNormal.x), Mathf.Abs(ResultNormal.y), Mathf.Abs(ResultNormal.z));
        Vector2 OctNormal = (Vector2)ResultNormal / Vector3.Dot(Vector3.one, absVec);
        if (ResultNormal.z <= 0)
        {
            float absY = Mathf.Abs(OctNormal.y);
            float value = (1 - absY) * (OctNormal.y >= 0 ? 1 : -1);
            OctNormal = new(value, value);
        }
        return OctNormal;
    }

    public static Vector3[] ComputeSmoothedNormals3(Mesh mesh)
    {
        Vector3[] verts = mesh.vertices;
        Vector3[] nors = mesh.normals;
        Vector4[] tans = mesh.tangents;

        int vertexCount = verts.Length;
        if (tans.Length == 0)
        {
            return null;
        }

        Dictionary<Vector3, Vector3> smoothedNormalsMap = new(vertexCount);
        for (int i = 0; i < vertexCount; i++)
        {
            if (smoothedNormalsMap.TryGetValue(verts[i], out Vector3 normal))
            {
                smoothedNormalsMap[verts[i]] = normal + nors[i];
            }
            else
            {
                smoothedNormalsMap.Add(verts[i], nors[i]);
            }
        }

        Vector3[] bakedSmoothedNormals = new Vector3[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            bakedSmoothedNormals[i] = smoothedNormalsMap[verts[i]].normalized;
        }
        return bakedSmoothedNormals;
    }
}
}
