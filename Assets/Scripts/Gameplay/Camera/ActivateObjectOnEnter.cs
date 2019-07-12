using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjectOnEnter : MonoBehaviour {

    public GameObject m_objectToActivate;

    void OnTriggerEnter(Collider other)
    {
        if (m_objectToActivate)
        {
            m_objectToActivate.SetActive(true);
        }
    }
}
