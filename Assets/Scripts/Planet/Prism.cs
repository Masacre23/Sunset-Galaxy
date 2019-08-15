using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Prism {

    public static void Create(GameObject gameObject, Vector3[] verticesOfPlanet, Vector3[] oppositeVertices, bool inverseNormals = false, bool withoutCollider = false) {

        Vector3 p0 = verticesOfPlanet[0];
        Vector3 p1 = verticesOfPlanet[1];
        Vector3 p2 = oppositeVertices[0];
        Vector3 p3 = oppositeVertices[1];
        Vector3 p4 = verticesOfPlanet[2];
        Vector3 p5 = oppositeVertices[2];

        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        mesh.vertices = new Vector3[18]{
            //Bottom
            p0, p1, p2, p3,
            //Right
            p1, p3, p4, p5,
            //Left
            p4, p5, p0, p2,
            //Front
            p0, p1, p4,
            //Back
            p2, p3, p5
        };


        float w = gameObject.transform.localScale.x,
                    h = gameObject.transform.localScale.y,
                    d = gameObject.transform.localScale.z,
                    hyp = 1.414f;

        mesh.uv = new Vector2[18] {
            new Vector2( .5f*w, -.5f*d),
                new Vector2(-.5f*w, -.5f*d),
                new Vector2( .5f*w,  .5f*d),
                new Vector2(-.5f*w,  .5f*d),

                new Vector2(hyp*h, -.5f*d),
                new Vector2(hyp*h,  .5f*d),
                new Vector2(0f,    -.5f*d),
                new Vector2(0f,     .5f*d),

                new Vector2( 0f,    -.5f*d),
                new Vector2( 0f,     .5f*d),
                new Vector2(-hyp*h, -.5f*d),
                new Vector2(-hyp*h,  .5f*d),

                new Vector2(-.5f*w, -.5f*h),
                new Vector2( .5f*w, -.5f*h),
                new Vector2(  0f,    .5f*h),

                new Vector2( .5f*w, -.5f*h),
                new Vector2(-.5f*w, -.5f*h),
                new Vector2(  0f,    .5f*h)
        };

        mesh.triangles = new int[24]
        {
            0, 1, 2, 1, 3, 2,
            6, 5, 4, 6, 7, 5,
            10, 9, 8, 10, 11, 9,
            14, 13, 12,
            15, 16, 17
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        if (inverseNormals) {
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

            for (int m = 0; m < mesh.subMeshCount; m++) {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3) {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }

        if(!withoutCollider)
            (gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider).sharedMesh = mesh;
    }


    public static void SetColors(this Mesh mesh, Color topColor, Color defaultColor) {
        Color[] colors = new Color[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++) {
            if (i > mesh.vertices.Length - 6)
                colors[i] = topColor;
            else
                colors[i] = defaultColor;
            //colors[i] = Color.Lerp(colorDirt, colorGrass, planetMesh.vertices[i].y);
        }

        mesh.colors = colors;
    }
}
