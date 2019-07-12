using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetectorByTag : MonoBehaviour {

    public string m_tag;
    public List<GameObject> m_targets;

    void Awake()
    {
        m_targets = new List<GameObject>();
    }

    public GameObject GetNearestTarget(Vector3 origin, GameObject ignore = null)
    {
        GameObject nearest = null;
        float nearDistance2 = 1000.0f;

        foreach (GameObject target in m_targets)
        {
            float distance2 = (target.transform.position - origin).sqrMagnitude;
            if (distance2 < nearDistance2 && target != ignore)
            {
                nearest = target;
                nearDistance2 = distance2;
            }
        }

        return nearest;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag(m_tag))
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
