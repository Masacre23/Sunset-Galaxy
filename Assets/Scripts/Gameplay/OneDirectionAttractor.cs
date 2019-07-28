using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneDirectionAttractor : GravityAttractor
{
    BoxCollider m_collider;
    Vector3 m_gravityNormal;
    public Vector3 m_gravityNormalLocal;
    public bool m_recalculateGravityOnUpdate = false;

    public override void Start()
    {
        m_collider = GetComponent<BoxCollider>();
        if (!m_collider)
            m_collider = gameObject.AddComponent<BoxCollider>();

        if (m_gravityNormalLocal == Vector3.zero)
            m_gravityNormalLocal = Vector3.up;

        m_gravityNormalLocal.Normalize();
        m_gravityNormal = transform.TransformDirection(m_gravityNormalLocal);

        base.Start();
    }

    public override void Update()
    {
        if (m_recalculateGravityOnUpdate)
        {
            m_gravityNormal = transform.TransformDirection(m_gravityNormalLocal);
        }
    }

    public override void GetDistanceAndGravityVector(Vector3 position, ref Vector3 gravity, ref float distance)
    {
        gravity = m_gravityNormal;
        distance = Mathf.Abs(Vector3.Dot(position - transform.position, m_gravityNormal));
    }

    public override float GetDistance(Vector3 position)
    {
        return Vector3.Dot(position - transform.position, m_gravityNormal);
    }

    public override Vector3 GetGravity(Vector3 position)
    {
        return m_gravityNormal;
    }

    public override Vector3 GetGravityWithDistance(Vector3 position)
    {
        float distance = Vector3.Dot(position - transform.position, m_gravityNormal);

        if (distance > 0.0f)
            return distance * m_gravityNormal;
        else
            return Vector3.zero;

    }
}