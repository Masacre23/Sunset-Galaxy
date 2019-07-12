using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour {

    /*public float turnSpeed = 4.0f;
    public Transform player;

    public float height = 1f;
    public float distance = 2f;

    private Vector3 offsetX;

    void Start() {

        offsetX = new Vector3(0, height, distance);
    }

    void LateUpdate() {
        offsetX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetX;
        transform.position = player.position + offsetX;
        transform.LookAt(player.position);
    }*/

    /*public float turnSpeed = 4.0f;
    public Transform player;
    public float height = 1f;
    public float distance = 2f;

    private Vector3 offsetX;
    private Vector3 offsetY;

    void Start() {

        offsetX = new Vector3(0, height, distance);
        offsetY = new Vector3(0, 0, distance);
    }

    void LateUpdate() {
        offsetX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetX;
        offsetY = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeed, Vector3.right) * offsetY;
        transform.position = player.position + offsetX + offsetY;
        transform.LookAt(player.position);
    }*/
    public float yDist = 10;
    public float zDist = -5;

    public float rotationSpeed = 2f;
    public Transform player;
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    void LateUpdate() {
        transform.position = player.position;
        transform.Rotate(0f, Input.GetAxis("X") * Time.deltaTime * rotationSpeed, 0f);
    }
    void OnGUI() {
        if (GUI.Button(new Rect(Screen.width - 200f, 20f, 180f, 40f), "Spaceship level")) {
            Application.LoadLevel(1);
        }
    }
}