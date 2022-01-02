using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour {
    public enum PlanetMaterial { DIRT, WATER, LAVA }
    public PlanetGenerator planet;
    public Camera cam;
    public GameObject waterDrop;
    public GameObject lavaDrop;
    public PlanetMaterial currentMaterial = PlanetMaterial.DIRT;
    public float delay = 0.5f;
    float count = 0f;
    bool spawnEnabled = false;
    GameObject playerCamera;
    GameObject flyingCamera;
    GameObject firstPersonCamera;

    private void Start() {
        playerCamera = GameObject.Find("MainCameraRig");
        flyingCamera = GameObject.Find("FlyingCamera");
        flyingCamera.SetActive(false);
        firstPersonCamera = GameObject.Find("FPCamera");
    }

    public void SetMaterial(string newMaterial) {
        currentMaterial = (PlanetMaterial)System.Enum.Parse(typeof(PlanetMaterial), newMaterial);
    }
	
	void FixedUpdate () {
        if (spawnEnabled && count > delay) {
            count = 0f;
            Spawn();
        }
        if(currentMaterial != PlanetMaterial.DIRT)
            count += Time.fixedDeltaTime;
        count += Time.fixedDeltaTime;
	}

    private void Spawn() {
        RaycastHit hit;
        //Debug.DrawLine(cam.gameObject.transform.position, cam.ScreenPointToRay(Input.mousePosition).direction);
        //if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
        // Debug.DrawLine(cam.transform.position, cam.transform.position + cam.transform.forward * 10, Color.red);
        if (!Physics.Raycast(cam.transform.position, cam.transform.forward * 100, out hit)) {
            return;
        }
        

        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null) {
            return;
        }

        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);

        Vector3 initPos = cam.transform.position + cam.transform.forward * 2;
        switch (currentMaterial) {
            case PlanetMaterial.DIRT:
                GameObject block = planet.CreatePrismGameObject(new Vector3[] { p0, p1, p2 }, hit.collider.gameObject.GetComponent<PrismData>(), prismPosition: PrismData.PrismPosition.UP);
                Vector3 finalPos = block.transform.position;
                StartCoroutine(LaunchBlock(block.transform, initPos, finalPos, 5f));
                break;
            default:
                StartCoroutine(LaunchMaterial(initPos, planet.transform.position, 10f));
                break;
        }

        Debug.DrawLine(p0, p1);
        Debug.DrawLine(p1, p2);
        Debug.DrawLine(p2, p0);
    }

    public void OnClick() {
        spawnEnabled = !spawnEnabled;
    }

    IEnumerator LaunchBlock(Transform objectToMove, Vector3 initPos, Vector3 finalPos, float speed) {
        Transform flyingObject = Instantiate(objectToMove, initPos, Quaternion.identity);
        flyingObject.localScale *= planet.planetSize;
        flyingObject.GetComponent<MeshCollider>().enabled = false;
        objectToMove.GetComponent<MeshRenderer>().enabled = false;
        float step = (speed / (initPos - finalPos).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f) {
            t += step; // Goes from 0 to 1, incrementing by step each time
            flyingObject.position = Vector3.Lerp(initPos, finalPos, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
        flyingObject.gameObject.SetActive(false);
        //Destroy(flyingObject);
        objectToMove.GetComponent<MeshRenderer>().enabled = true;
    }

    IEnumerator LaunchMaterial(Vector3 initPos, Vector3 finalPos, float speed) {
        GameObject material;
        if (currentMaterial == PlanetMaterial.WATER)
            material = Instantiate(waterDrop, initPos, Quaternion.identity);
        else
            material = Instantiate(lavaDrop, initPos, Quaternion.identity);
        float step = (speed / (initPos - finalPos).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f) {
            t += step; // Goes from 0 to 1, incrementing by step each time
            material.transform.position = Vector3.Lerp(initPos, finalPos, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
        //Destroy(flyingObject);
        material.transform.position = finalPos;
        material.SetActive(false);
        if (currentMaterial == PlanetMaterial.WATER)
            planet.IncreaseWater();
        else
            planet.DecreaseWater();
    }

    public void ChangeCamera() {
        playerCamera.SetActive(!playerCamera.activeInHierarchy);
        firstPersonCamera.SetActive(!firstPersonCamera.activeInHierarchy);
        flyingCamera.SetActive(!flyingCamera.activeInHierarchy);

        Cursor.visible = !Cursor.visible;
        if (Cursor.visible)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    public void Jump() {
        GameObject.Find("Player").GetComponent<Player>().m_jumping = true;
    }
}
