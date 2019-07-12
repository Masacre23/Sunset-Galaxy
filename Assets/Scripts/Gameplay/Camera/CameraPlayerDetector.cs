using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerDetector : MonoBehaviour {
    public GameObject m_target;
    public bool m_playerInside { get; private set; }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == m_target) {
            m_playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == m_target) {
            m_playerInside = false;
        }
    }
}