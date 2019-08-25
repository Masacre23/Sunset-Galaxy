using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using ExtensionMethods;
//using static ExtensionMethods.MyExtensions;
using System.Linq.Expressions;
using System;

public class PrismGenerator {

    Color highlightColor = Color.red;
    Color previousColor;
    private Mesh lastMesh;
    private Vector3 lastHitNormal;
    PlanetGenerator planet;
    bool addBlock = false;
    public GameObject previsualizationPrism;

    public PrismGenerator(PlanetGenerator planet) {
        this.planet = planet;
    }

    public void MobileBlock() {
        addBlock = true;
    }

    Vector3 initTest = Vector3.zero;
    Vector3 finalTest = Vector3.zero;
    void Update() {
        /*Debug.DrawLine(initTest, finalTest, Color.green);
        RaycastHit hit;
        if (cam) {
            Debug.DrawLine(cam.transform.position, cam.transform.forward * 100, Color.red);
            if (Physics.Raycast(cam.transform.position, cam.transform.forward * 100, out hit)) {
                HighlightSelectedFace(hit);
            } else {
                RemoveHighlights();
            }
        } else {
            Debug.DrawLine(gameObject.transform.position, (gameObject.transform.forward - gameObject.transform.up) * 100, Color.red);
            if (Physics.Raycast(gameObject.transform.position, (gameObject.transform.forward - gameObject.transform.up) * 100, out hit)) {
                HighlightSelectedFace(hit);
            } else {
                RemoveHighlights();
            }
        }*/
    }

    private Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c) {
        // Find vectors corresponding to two of the sides of the triangle.
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;

        // Cross the vectors to get a perpendicular vector, then normalize it.
        return Vector3.Cross(side1, side2).normalized;
    }

    public void HighlightSelectedFace(RaycastHit hit) {
        if (lastHitNormal != Vector3.zero) {
            RemoveHighlights();
        }
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;
        bool isPuttingPrism = CrossPlatformInputManager.GetButtonDown("Fire1") || addBlock;
        if (isPuttingPrism || hit.collider.gameObject != previsualizationPrism) {

            GameObject newPrism = null;
            if (hit.collider.gameObject.transform.position.normalized.isAproximated(hit.normal, 0.1f) || hit.collider.gameObject.transform.parent == planet.transform)
                newPrism = PutBlock(vertices, triangles, hit, hit.collider.gameObject.GetComponent<PrismData>(), PrismData.PrismPosition.UP, isPrevisualization: !isPuttingPrism);
            else if (hit.collider.gameObject.transform.position.normalized.isAproximated(-hit.normal, 0.1f)) {
                newPrism = PutBlock(vertices, triangles, hit, hit.collider.gameObject.GetComponent<PrismData>(), PrismData.PrismPosition.DOWN, true, !isPuttingPrism);
            } else {
                Vector3 mediumPointOfPrism = Vector3.zero;
                foreach(Vector3 v in vertices) {
                    mediumPointOfPrism += v;
                }
                mediumPointOfPrism = mediumPointOfPrism / vertices.Length;

                float distBetweenVertices = Vector3.Distance(vertices[0], vertices[1]);

                RaycastHit hit2;
                initTest = hit.collider.gameObject.transform.position + mediumPointOfPrism + hit.normal.normalized * (distBetweenVertices/2 + 0.001f);
                finalTest = (planet.transform.position - hit.collider.gameObject.transform.position).normalized * 100;

                if (Physics.Raycast(hit.collider.gameObject.transform.position + mediumPointOfPrism + hit.normal.normalized * (distBetweenVertices/2 + 0.001f), (planet.transform.position - hit.collider.gameObject.transform.position).normalized * 100, out hit2)) {
                    MeshCollider mc = hit2.collider as MeshCollider;
                    if (meshCollider == null || meshCollider.sharedMesh == null)
                        return;

                    newPrism = PutBlock(mc.sharedMesh.vertices, mc.sharedMesh.triangles, hit2, hit.collider.gameObject.GetComponent<PrismData>(), PrismData.PrismPosition.AROUND, isPrevisualization: !isPuttingPrism);
                }

                //return;
            }

            if (previsualizationPrism != null)
                previsualizationPrism.SetActive(false);

            if (!isPuttingPrism){
                previsualizationPrism = newPrism;
            }
        }
        Color[] colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++) {
            if (normals[i] == hit.normal) {
                previousColor = mesh.colors[i];
                colors[i] = highlightColor;
            } else {
                colors[i] = mesh.colors[i];
            }
        }

        mesh.colors = colors;
        lastMesh = mesh;
        lastHitNormal = hit.normal;
    }

    private Vector3 GetThirdVertexOfTriangle(Vector3 oppositeVertex, Vector3 v1, Vector3 v2, Vector3 prismaPosReference, Vector3 normal, bool with = true) {
        Vector3 ret = Vector3.zero;

        float dist1X = v1.x - oppositeVertex.x;
        float dist1Y = v1.y - oppositeVertex.y;
        float dist1Z = v1.z - oppositeVertex.z;

        float dist2X = v2.x - oppositeVertex.x;
        float dist2Y = v2.y - oppositeVertex.y;
        float dist2Z = v2.z - oppositeVertex.z;

        Vector3 point1 = v1 + new Vector3(dist1X, dist1Y, dist1Z);
        Vector3 point2 = v2 + new Vector3(dist2X, dist2Y, dist2Z);

        ret = (point1 + point2) / 2f + prismaPosReference;


        Vector3 dirV1ToOppositeVertex = (oppositeVertex - (v2 - v1) / 2).normalized;
        Vector3 dirV1ToDestPoint = -dirV1ToOppositeVertex;//Vector3.Cross(dirV1ToOppositeVertex, Vector3.Normalize(normal));

        ret = v1 + Vector3.Distance(v1, oppositeVertex) * dirV1ToDestPoint + prismaPosReference;

        /*print(dirV1ToOppositeVertex);
        print(Vector3.Normalize(normal));
        print(dirV1ToDestPoint);*/

        float correctDistanceToPlanet = Vector3.Distance(planet.transform.position, v1 + prismaPosReference);
        float actualDistanceToPlanet = Vector3.Distance(planet.transform.position, ret);
        float distToAproachToPlanet = actualDistanceToPlanet - correctDistanceToPlanet;
        Vector3 dirToPlanet = (planet.transform.position - ret).normalized;

        if (with)
            ret += dirToPlanet * distToAproachToPlanet;

        return ret;
    }

    void RemoveHighlights() {
        if (lastMesh) {
            Mesh mesh = lastMesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;
            Color[] originalColors = mesh.colors;

            for (var i = 0; i < vertices.Length; i++) {
                if (normals[i] == lastHitNormal) {
                    originalColors[i] = previousColor;
                }
            }

            mesh.colors = originalColors;

            lastMesh = null;
            lastHitNormal = Vector3.zero;
            if(previsualizationPrism != null)
                previsualizationPrism.SetActive(false);
            previsualizationPrism = null;
        }
    }

    GameObject PutBlock(Vector3[] vertices, int[] triangles, RaycastHit hit, PrismData data, PrismData.PrismPosition prismPos, bool inverseNormals = false, bool isPrevisualization = false) {

        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);

        return planet.CreatePrismGameObject(new Vector3[] { p0, p1, p2 }, data, hit.collider.gameObject.GetComponent<PrismData>(), prismPos, inverseNormals, isPrevisualization);
    }
}
