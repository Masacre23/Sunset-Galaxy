using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using ExtensionMethods;

public class ObjectSpawner : MonoBehaviour
{
    public enum Type { PRISM, OTHER }
    public Type type = Type.PRISM;
    public GameObject objectToSpawn;
    PrismGenerator prismGenerator;
    GameObject previsualizationObject;
    public Material previsualizationMaterial;

    void Start() {
        prismGenerator = new PrismGenerator(GameObject.Find("Planet").GetComponent<PlanetGenerator>());
    }

    void Update() {
        if (CrossPlatformInputManager.GetButtonDown("Fire2"))
        {
            type = Type.OTHER;
        }

        RaycastHit hit;
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.forward * 100, Color.red);
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * 100, out hit)) {
            switch (type) {
                case Type.PRISM:
                    SpawnPrism(hit);
                    break;
                case Type.OTHER:
                    SpawnObject(hit);
                    break;
            }
        }
    }

    void SpawnObject(RaycastHit hit)
    {
        prismGenerator.previsualizationPrism.SetActive(false);

        GameObject go = Instantiate(objectToSpawn, Vector3.zero, Quaternion.identity);

        GameObject floor = hit.collider.gameObject;
        go.transform.LookAt(floor.transform);
        go.transform.Rotate(new Vector3(90, 0, 0));
        Collider col = go.GetComponent<Collider>();

        Vector3 dir = (floor.transform.position - go.transform.position).normalized;
        go.transform.position = floor.transform.position + dir;

        if(previsualizationObject)
            previsualizationObject.SetActive(false);

        if (!CrossPlatformInputManager.GetButtonDown("Fire1")) { 
            previsualizationObject = go;
            for(int i = 0; i < previsualizationObject.transform.childCount; i++) {
                previsualizationObject.transform.GetChild(i).GetComponent<Collider>().isTrigger = true;
                MeshRenderer meshRenderer = previsualizationObject.transform.GetChild(i).GetComponent<MeshRenderer>();
                meshRenderer.material = previsualizationMaterial;
            }
        }
    }

    void SpawnPrism(RaycastHit hit) {
        prismGenerator.HighlightSelectedFace(hit);
    }
}
