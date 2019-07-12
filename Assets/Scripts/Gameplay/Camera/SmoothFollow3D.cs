using System.Collections;
using System.Collections.Generic;
using UnityEngine;
  
 public class SmoothFollow3D : MonoBehaviour {
    public Transform target;
    public Transform reference;
    public float upDistance = 0.5f;
    public float backDistance = 0.75f;
    public float trackingSpeed = 0.5f;
    public float rotationSpeed = 1.0f;

    private Vector3 v3To;
    private Quaternion qTo;

    Vector3 m_PrevMousePosition;
    public float m_MouseDragSensitivity = 1.0f;
    public float m_MouseZoomSensitivity = 1.0f;

    void LateUpdate() {
       /* Vector3 v3Up = (target.position - reference.position).normalized;
        v3To = target.position - target.forward * backDistance + v3Up * upDistance;
        transform.position = Vector3.Lerp(transform.position, v3To, trackingSpeed * Time.deltaTime);*/

        //qTo = Quaternion.LookRotation(target.position - transform.position, v3Up);
        //transform.rotation = Quaternion.Slerp(transform.rotation, qTo, rotationSpeed * Time.deltaTime);

        Vector3 currentMousePosition = Input.mousePosition;
        if (Input.GetMouseButton(0)) {
            // So long as the mouse button is held down, track how far it has been moved in screen-space each frame.

            Vector3 mouseDisplacement = currentMousePosition - m_PrevMousePosition;

            mouseDisplacement.x /= Screen.width;
            mouseDisplacement.y /= Screen.height;

            // Moving the mouse left and right results in a rotation around the camera's own up axis.

            Quaternion yaw = Quaternion.AngleAxis(mouseDisplacement.x * m_MouseDragSensitivity, transform.up);

            // And moving the mouse up and down results in a rotation around the camera's right axis.

            Quaternion pitch = Quaternion.AngleAxis(mouseDisplacement.y * m_MouseDragSensitivity, -transform.right);

            // Since we have world-space rotations that we want to apply to the camera transform, we need to
            // multiply from the left. As in "yaw * transform.localRotation" rather than "transform.localRotation * yaw".

            transform.localRotation = yaw * pitch * transform.localRotation;
        }
    }
}