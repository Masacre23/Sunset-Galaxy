using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageData
{
    public bool m_recive;
    public int m_damage;
    public bool m_respawn;
    public bool m_alive;
    public Vector3 m_force;

    public DamageData()
    {
        m_recive = false;
        m_damage = 0;
        m_respawn = false;
        m_alive = true;
        m_force = Vector3.back + Vector3.up;
    }

    public void ResetDamageData()
    {
        m_recive = false;
        m_damage = 0;
        m_respawn = false;
        m_alive = true;
        m_force = Vector3.back + Vector3.up;
    }
}
