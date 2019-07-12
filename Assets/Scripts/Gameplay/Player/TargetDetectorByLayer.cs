using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetectorByLayer : MonoBehaviour
{
    public LayerMask m_targetLayers;
    public List<GameObject> m_targets;

    void Awake()
    {
        m_targets = new List<GameObject>();
    }

    void OnTriggerEnter(Collider col)
    {
        //Check if the collision collider layer is in m_targetLayers
        if (m_targetLayers == (m_targetLayers | (1 << col.gameObject.layer)))
        {
            m_targets.Add(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (m_targets.Contains(col.gameObject))
        {
            m_targets.Remove(col.gameObject);
        }
    }
}
