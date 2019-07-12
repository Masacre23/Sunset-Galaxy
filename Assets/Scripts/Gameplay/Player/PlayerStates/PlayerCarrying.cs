using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarrying : PlayerStates
{
    public bool m_pickingOrThrowing = false;
    public bool m_hasThrown = false;
    public float m_floatingHeight = 1.0f;

    public override void Start()
    {
        base.Start();
        m_type = States.CARRYING;
    }

    //Main player update. Returns true if a change in state ocurred (in order to call OnExit() and OnEnter())
    public override bool OnUpdate(float axisHorizontal, float axisVertical, bool jumping, bool pickObjects, bool aimingObject, float timeStep)
    {
        bool ret = false;

        m_player.CheckPlayerStopped(axisHorizontal, axisVertical);

        m_player.UpdateUp();

        if (!m_pickingOrThrowing)
        {
            m_player.Move(timeStep);

            if (pickObjects)
            {
                m_player.ThrowObjectsThirdPerson();
                m_pickingOrThrowing = true;
            }
        }

        if (!m_player.CheckGroundStatus())
        {
            m_player.m_currentState = m_player.m_onAir;
            ret = true;
        }

        if (m_hasThrown)
        {
            m_player.m_currentState = m_player.m_grounded;
            ret = true;
        }

        return ret;
    }

    public override void OnEnter()
    {
        m_player.m_rotationFollowPlayer = true;
        m_player.m_freezeMovement = false;

        m_hasThrown = false;

        m_player.m_jumpMovement = Vector3.zero;
        m_player.m_rigidBody.velocity = Vector3.zero;
        m_player.m_animator.SetBool("Charging", true);
    }

    public override void OnExit()
    {
        m_player.m_animator.SetBool("Charging", false);
    }
}
