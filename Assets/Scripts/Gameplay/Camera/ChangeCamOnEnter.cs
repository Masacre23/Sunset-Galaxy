using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamOnEnter : MonoBehaviour
{
    public GameObject m_orientation;
    public float m_transitionToLook = 0.5f;
    Collider m_collider;

	void Start ()
    {
        m_collider = GetComponent<Collider>();
        if (!m_orientation)
        {
            m_orientation = gameObject;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            player.m_camController.SetCameraOnBackLookingAtTarget(m_orientation.transform, m_transitionToLook);
            gameObject.SetActive(false);
        }
    }
}
