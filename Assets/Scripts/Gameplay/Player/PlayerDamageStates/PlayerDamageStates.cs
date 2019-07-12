using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageStates : MonoBehaviour {

    public enum States
    {
        VULNERABLE,
        INVULNERABLE,
        RECEIVING,
        DEAD
    }

    public States m_type;
    protected Player m_player;

    public virtual void Start ()
    {
        m_player = GetComponent<Player>();
    }

    public virtual bool OnUpdate(DamageData data)
    {
        return false;
    }

    public virtual void OnEnter(DamageData data)
    {
    }

    public virtual void OnExit(DamageData data)
    {
    }
}
