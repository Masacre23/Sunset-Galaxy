using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour {
    public Material groundMaterial;
    public Material waterMaterial;
    public float planetSize = 1f;
    GameObject planet;
    GameObject water;
    GameObject lava;
    Mesh planetMesh;
    Vector3[] planetVertices;
    int[] planetTriangles;
    MeshRenderer planetMeshRenderer;
    MeshFilter planetMeshFilter;
    MeshCollider planetMeshCollider;

    Color32 colorOcean = new Color32(0, 80, 220, 0);
    Color32 colorLava = new Color32(220, 80, 0, 0);
    Color32 colorGrass = new Color32(0, 220, 0, 0);
    Color32 colorDirt = new Color32(180, 140, 20, 0);
    Color32 colorDeepOcean = new Color32(0, 40, 110, 0);

    float prismSize = 0.1f;

    private void Awake() {
        CreatePlanet();
    }

    public void CreatePlanet() {
        CreatePlanetGameObject();
        CreateWaterGameObject();
        CreateLavaGameObject();
        
        //do whatever else you need to do with the sphere mesh
        RecalculateMesh();
    }

    public void IncreaseWater() {
        water.transform.localScale *= 1.01f;
        lava.transform.localScale *= .99f;
    }

    public void DecreaseWater() {
        water.transform.localScale *= .99f;
        lava.transform.localScale *= 1.01f;
    }

    void CreatePlanetGameObject() {
        planet = new GameObject();
        planetMeshFilter = planet.AddComponent<MeshFilter>();
        planetMesh = planetMeshFilter.mesh;
        planetMeshRenderer = planet.AddComponent<MeshRenderer>();

        //need to set the material up top
        planetMeshRenderer.material = groundMaterial;
        planet.transform.localScale = new Vector3(planetSize, planetSize, planetSize);
        IcoSphere.Create(planet);

        Color[] colors = new Color[planetMesh.vertices.Length];
        for (int i = 0; i < planetMesh.vertices.Length; i++)
            colors[i] = colorDeepOcean;

        planetMesh.colors = colors;
        (planet.AddComponent(typeof(MeshCollider)) as MeshCollider).sharedMesh = planetMesh;
        planet.transform.parent = gameObject.transform;
    }

    void CreateWaterGameObject() {
        water = new GameObject();
        water.AddComponent<MeshFilter>();
        water.AddComponent<MeshRenderer>().material = waterMaterial;
       

        //need to set the material up top
        water.transform.localScale = new Vector3(planetSize * 1.1f + 0.1f, planetSize * 1.1f + 0.1f, planetSize * 1.1f + 0.1f);
        IcoSphere.Create(water);
        Mesh mesh = water.GetComponent<MeshFilter>().mesh;
        Color[] colors = new Color[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
            colors[i] = colorOcean;//Color.Lerp(Color.red, Color.green, planetMesh.vertices[i].y);

        mesh.colors = colors;
        water.transform.parent = gameObject.transform;
    }

    void CreateLavaGameObject() {
        lava = new GameObject();
        lava.AddComponent<MeshFilter>();
        lava.AddComponent<MeshRenderer>().material = waterMaterial;


        //need to set the material up top
        lava.transform.localScale = new Vector3(planetSize * 0.9f, planetSize * 0.9f, planetSize * 0.9f);
        IcoSphere.Create(lava);
        Mesh mesh = lava.GetComponent<MeshFilter>().mesh;
        Color[] colors = new Color[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
            colors[i] = colorLava;//Color.Lerp(Color.red, Color.green, planetMesh.vertices[i].y);

        mesh.colors = colors;
        lava.transform.parent = gameObject.transform;
    }

    public GameObject CreatePrismGameObject(Vector3[] vertices, PrismData firstHitPrismData = default(PrismData), PrismData downPrismData = default(PrismData), PrismData.PrismPosition prismPosition = PrismData.PrismPosition.DOWN, bool inverseNormals = false) {
        GameObject prism = new GameObject();
        int height = 0;
        // if (downPrismData != null) {
        switch (prismPosition) {
                case PrismData.PrismPosition.DOWN:
                    //height = firstHitPrismData.height - 1;
                    vertices = new Vector3[]{
                        vertices[0] - (vertices[0] - planet.gameObject.transform.position) * prismSize / 1.1f, //1.1f es un numero mágico pero funciona xD
                        vertices[1] - (vertices[1] - planet.gameObject.transform.position) * prismSize / 1.1f,
                        vertices[2] - (vertices[2] - planet.gameObject.transform.position) * prismSize / 1.1f
                    };
                    print(vertices.toString());
                    break;
                case PrismData.PrismPosition.UP:
                   /* vertices = new Vector3[]{
                        vertices[0] - (vertices[0] - planet.gameObject.transform.position) * prismSize,
                        vertices[1] - (vertices[1] - planet.gameObject.transform.position) * prismSize,
                        vertices[2] - (vertices[2] - planet.gameObject.transform.position) * prismSize
                    };*/
                    break;
                case PrismData.PrismPosition.AROUND:
                    
                    if (downPrismData == null)
                        height = firstHitPrismData.height;
                    else
                        height = (firstHitPrismData.height - downPrismData.height - 1);

                    (height - 1).Times(() => {
                        vertices = new Vector3[]{
                            vertices[0] + (vertices[0] - planet.gameObject.transform.position) * prismSize,
                            vertices[1] + (vertices[1] - planet.gameObject.transform.position) * prismSize,
                            vertices[2] + (vertices[2] - planet.gameObject.transform.position) * prismSize
                        };
                    });
                    break;
            }   
        //}

        Vector3[] oppositeVertices = {
            vertices[0] + (vertices[0] - planet.gameObject.transform.position)*prismSize,
            vertices[1] + (vertices[1] - planet.gameObject.transform.position)*prismSize,
            vertices[2] + (vertices[2] - planet.gameObject.transform.position)*prismSize
        };

        /*prism.transform.position = new Vector3(
            (vertices[0].x + vertices[1].x + vertices[2].x + oppositeVertices[0].x + oppositeVertices[1].x + oppositeVertices[2].x) / 6,
            (vertices[0].y + vertices[1].y + vertices[2].y + oppositeVertices[0].y + oppositeVertices[1].y + oppositeVertices[2].y) / 6,
            (vertices[0].z + vertices[1].z + vertices[2].z + oppositeVertices[0].z + oppositeVertices[1].z + oppositeVertices[2].z) / 6
            );*/

        prism.transform.position = new Vector3(
            (vertices[0].x + vertices[1].x + vertices[2].x) / 3,
            (vertices[0].y + vertices[1].y + vertices[2].y) / 3,
            (vertices[0].z + vertices[1].z + vertices[2].z) / 3
            );

        Mesh mesh = prism.AddComponent<MeshFilter>().mesh;
        prism.AddComponent<MeshRenderer>().material = groundMaterial;

        Vector3[] verticesRelatedToParent = {
            vertices[0] - prism.transform.position,
            vertices[1] - prism.transform.position,
            vertices[2] - prism.transform.position
        };

        Vector3[] oppositeVerticesRelatedToParent = {
            oppositeVertices[0] - prism.transform.position,
            oppositeVertices[1] - prism.transform.position,
            oppositeVertices[2] - prism.transform.position
        };

        Prism.Create(prism, verticesRelatedToParent, oppositeVerticesRelatedToParent, inverseNormals);
        prism.transform.parent = planet.transform;

        mesh.SetColors(colorGrass, colorDirt);

        PrismData data = prism.AddComponent<PrismData>();
        /*   print(prism.transform.position);
           print(planet.transform.parent.transform.localScale);
           print(prism.transform.position - planet.transform.parent.transform.localScale / 2);
           //print((prism.transform.position - planet.transform.parent.transform.localScale/2).sqrMagnitude);

           Vector3 dist = (prism.transform.position - planet.transform.parent.transform.localScale / 2);
           if (dist.x < 0) dist.x = 0;
           if (dist.y < 0) dist.y = 0;
           if (dist.z < 0) dist.z = 0;
           print(dist);
           print(dist.magnitude);
           print(dist.magnitude);*/
        switch (prismPosition) {
            case PrismData.PrismPosition.DOWN:
                //print(oppositeVertices.toString());
                data.prismUp = firstHitPrismData.gameObject;
                data.height = firstHitPrismData.height - 1;
                break;
            case PrismData.PrismPosition.UP:
                // data.prismUp = downPrismData.gameObject;
                //data.height = downPrismData.height - 1;
                if (downPrismData == null) {
                    data.height = 0;
                } else {
                    data.prismDown = downPrismData.gameObject;
                    data.height = downPrismData.height + 1;
                }
                break;
            case PrismData.PrismPosition.AROUND:
                data.prismsAround = new GameObject[]{
                    firstHitPrismData.gameObject 
                };
                data.height = firstHitPrismData.height;
                break;
        }

        return prism;
    }

    void RecalculateMesh() {
        planetMesh.RecalculateBounds();
        planetMesh.RecalculateTangents();
        planetMesh.RecalculateNormals();
    }
}