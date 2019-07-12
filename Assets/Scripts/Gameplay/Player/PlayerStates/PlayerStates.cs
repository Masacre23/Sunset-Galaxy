using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    public enum States
    {
        GROUNDED,
        ONAIR,
        THROWING,
        CARRYING
    }

    public States m_type;
    protected Player m_player;
    protected Rigidbody m_rigidBody;
    protected Transform m_modelTransform;

    // Use this for initialization
    public virtual void Start ()
    {
        m_player = GetComponent<Player>();
        m_rigidBody = GetComponent<Rigidbody>();
        m_modelTransform = transform.Find("Model");
    }

    //Main player update. Returns true if a change in state ocurred (in order to call OnExit() and OnEnter())
    public virtual bool OnUpdate(float axisHorizontal, float axisVertical, bool jumping, bool pickObjects, bool aimingObject, float timeStep)
    {
        return false;
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }
}
