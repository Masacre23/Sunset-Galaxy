using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using ExtensionMethods;
//using static ExtensionMethods.MyExtensions;
using System.Linq.Expressions;
using System;

public class PrismGenerator : MonoBehaviour {

    Color highlightColor = Color.red;
    Color previousColor;
    private Mesh lastMesh;
    private Vector3 lastHitNormal;
    public Camera cam;
    public PlanetGenerator planet;
    bool addBlock = false;

    public void MobileBlock() {
        addBlock = true;
    }

    Vector3 initTest = Vector3.zero;
    Vector3 finalTest = Vector3.zero;
    void Update() {
        Debug.DrawLine(initTest, finalTest, Color.green);
        RaycastHit hit;
        //if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit)) {
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
        }
    }

    private Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c) {
        // Find vectors corresponding to two of the sides of the triangle.
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;

        // Cross the vectors to get a perpendicular vector, then normalize it.
        return Vector3.Cross(side1, side2).normalized;
    }

    void HighlightSelectedFace(RaycastHit hit) {
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
        if (CrossPlatformInputManager.GetButtonDown("Fire1") || addBlock) {
            /*  print("Yolo");
              print(hit.collider.gameObject.transform.position.normalized);
              print(hit.normal);
              print(hit.collider.gameObject.transform.position.normalized.isEqual(hit.normal, 10));
              */
            /* Vector3 v1 = hit.collider.gameObject.transform.position.normalized;
             Vector3 v2 = hit.normal;
             int x1 = (int)(v1.x * 10);
             print(x1);
             int y1 = (int)(v1.y * 10);
             int z1 = (int)(v1.z * 10);
             int x2 = (int)(v2.x * 10);
             print(x2);
             int y2 = (int)(v2.y * 10);
             int z2 = (int)(v2.z * 10);*/

            //print(triangles.toString());


            if (hit.collider.gameObject.transform.position.normalized.isAproximated(hit.normal, 0.1f) || hit.collider.gameObject.transform.parent == planet.transform)
                PutBlock(vertices, triangles, hit, hit.collider.gameObject.GetComponent<PrismData>(), PrismData.PrismPosition.UP);
            else if (hit.collider.gameObject.transform.position.normalized.isAproximated(-hit.normal, 0.1f)) {
                PutBlock(vertices, triangles, hit, hit.collider.gameObject.GetComponent<PrismData>(), PrismData.PrismPosition.DOWN, true);
            } else {
                Vector3 mediumPointOfPrism = Vector3.zero;
                foreach(Vector3 v in vertices) {
                    mediumPointOfPrism += v;
                }
                mediumPointOfPrism = mediumPointOfPrism / vertices.Length;

                float distBetweenVertices = Vector3.Distance(vertices[0], vertices[1]);


                RaycastHit hit2;
               /* print("Hit");
                print(hit.point + hit.normal);
                print((planet.transform.position - hit.collider.gameObject.transform.position).normalized * 100);*/
                initTest = hit.collider.gameObject.transform.position + mediumPointOfPrism + hit.normal.normalized * (distBetweenVertices/2 + 0.001f);
                finalTest = (planet.transform.position - hit.collider.gameObject.transform.position).normalized * 100;

                if (Physics.Raycast(hit.collider.gameObject.transform.position + mediumPointOfPrism + hit.normal.normalized * (distBetweenVertices/2 + 0.001f), (planet.transform.position - hit.collider.gameObject.transform.position).normalized * 100, out hit2)) {
                    MeshCollider mc = hit2.collider as MeshCollider;
                    if (meshCollider == null || meshCollider.sharedMesh == null)
                        return;

                    //print(mc.sharedMesh.vertices.toString());
                    //print(mc.sharedMesh.triangles);
                    

                    PutBlock(mc.sharedMesh.vertices, mc.sharedMesh.triangles, hit2, hit.collider.gameObject.GetComponent<PrismData>(), PrismData.PrismPosition.AROUND);
                }

                /*GameObject go = GameObject.Instantiate(hit.collider.gameObject, hit.collider.transform.parent);
                go.transform.position += hit.normal;
                go.transform.Rotate(Vector3.Cross(hit.normal, new Vector3(45, 0, 0)));*/

                return;


                for (int i = 0; i <= triangles.Length; i++) {
                    Vector3 v1 = vertices[triangles[i * 3 + 0]];
                    Vector3 v2 = vertices[triangles[i * 3 + 1]];
                    Vector3 v3 = vertices[triangles[i * 3 + 2]];

                    Vector3 dir = (planet.transform.position - hit.collider.gameObject.transform.position).normalized;

                    if (GetNormal(v1, v2, v3).isAproximated(dir, 0.1f)) {
                        //PutBlock(v1, v2, v3, triangles, hit);
                        Vector3 auxPoint = hit.collider.gameObject.transform.position + hit.normal.normalized * 10;
                        Vector3[] nearestPoints = new Vector3[] { v1, v2 };
                        Vector3[] allPoints = new Vector3[] { v1, v2, v3 };

                        2.Times(() => {
                            foreach (Vector3 v in allPoints) {
                                float dist = Vector3.Distance(auxPoint, v);

                                for (int j = 0; j < nearestPoints.Length; j++) {
                                    if (dist < Vector3.Distance(auxPoint, nearestPoints[j]) && !nearestPoints.Contains(v)) {
                                        nearestPoints[j] = v;
                                        break;
                                    }
                                }
                            }
                        });

                        Vector3 farPoint = Vector3.zero;
                        foreach (Vector3 v in allPoints) {
                            if (v != nearestPoints[0] && v != nearestPoints[1])
                                farPoint = v;
                        }

                        if (farPoint == v1) {
                            planet.CreatePrismGameObject(new Vector3[] {
                                GetThirdVertexOfTriangle(v1, v2, v3, hit.collider.gameObject.transform.position, hit.normal),
                                v2 + hit.collider.gameObject.transform.position,
                                v3 + hit.collider.gameObject.transform.position
                            });
                        } else if (farPoint == v2) {
                            planet.CreatePrismGameObject(new Vector3[] {
                                v1 + hit.collider.gameObject.transform.position/* + hit.normal*/,
                                GetThirdVertexOfTriangle(v2, v1, v3, hit.collider.gameObject.transform.position, hit.normal),
                                v3 + hit.collider.gameObject.transform.position
                            });
                        } else if (farPoint == v3) {
                            planet.CreatePrismGameObject(new Vector3[] {
                                v1 + hit.collider.gameObject.transform.position/* + hit.normal*/,
                                v2 + hit.collider.gameObject.transform.position,
                                GetThirdVertexOfTriangle(v3, v1, v2, hit.collider.gameObject.transform.position, hit.normal)
                            });
                        }

                        break;
                    }
                }
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

        print(dirV1ToOppositeVertex);
        print(Vector3.Normalize(normal));
        print(dirV1ToDestPoint);

        float correctDistanceToPlanet = Vector3.Distance(planet.transform.position, v1 + prismaPosReference);
        float actualDistanceToPlanet = Vector3.Distance(planet.transform.position, ret);
        float distToAproachToPlanet = actualDistanceToPlanet - correctDistanceToPlanet;
        Vector3 dirToPlanet = (planet.transform.position - ret).normalized;
        /*   print("V1 " + v1);
           print(correctDistanceToPlanet);
           print(actualDistanceToPlanet);
           print(dirToPlanet);
           print(ret);*/
        if (with)
            ret += dirToPlanet * distToAproachToPlanet;
        // print(ret);

        /*Vector3 mediumPoint = ((ret + v1) / 2 + planet.transform.position) / 2;

        ret = ret * correctDistanceToPlanet / actualDistanceToPlanet;*/

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
        }
    }

    void PutBlock(Vector3[] vertices, int[] triangles, RaycastHit hit, PrismData data, PrismData.PrismPosition prismPos, bool inverseNormals = false) {

        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);

        if (prismPos == PrismData.PrismPosition.DOWN) {
            print(p0);
            print(p1);
            print(p2);
        }

        planet.CreatePrismGameObject(new Vector3[] { p0, p1, p2 }, data, hit.collider.gameObject.GetComponent<PrismData>(), prismPos, inverseNormals);
    }
}
