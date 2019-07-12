using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCamTriggerTest : MonoBehaviour
{
    public Transform m_fixedCamPosition;
    public float m_transitionTime = 0.5f;
    bool m_playerInside = false;

	public void OnTriggerEnter(Collider other)
    {
        if (!m_playerInside && other.tag == "Player")
        {
            m_playerInside = true;
            other.GetComponent<Player>().m_camController.SetCameraOnFixed(m_fixedCamPosition, m_transitionTime);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (m_playerInside && other.tag == "Player")
        {
            m_playerInside = false;
            other.GetComponent<Player>().m_camController.SetCameraOnBack(m_transitionTime);
        }
    }

}
