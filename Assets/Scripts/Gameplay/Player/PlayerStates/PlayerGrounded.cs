using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrounded : PlayerStates
{
    public float m_floatingHeight = 1.0f;
    [Header("JumpCloud")]
    public GameObject m_smokeCloud;
    public Transform m_smokePosition;

    public override void Start()
    {
        base.Start();
        m_type = States.GROUNDED;
    }

    //Main player update. Returns true if a change in state ocurred (in order to call OnExit() and OnEnter())
    public override bool OnUpdate(float axisHorizontal, float axisVertical, bool jumping, bool pickObjects, bool aimingObject, float timeStep)
    {
        bool ret = false;

        m_player.CheckPlayerStopped(axisHorizontal, axisVertical);

        if (jumping || m_player.m_jumpOnEnemy)
        {
            m_player.Jump(axisHorizontal, axisVertical);

            m_player.m_currentState = m_player.m_onAir;
            ret = true;
        }
        else
        {
            m_player.UpdateUp();
            m_player.Move(timeStep);
            if (pickObjects)
            {
                if (m_player.TryToPick())
                {
                    m_player.m_currentState = m_player.m_carrying;
                    ret = true;
                }
            }
                
            if (!m_player.CheckGroundStatus())
            {
                m_player.m_currentState = m_player.m_onAir;
                ret = true;
            }
        }   

        return ret;
    }

    public override void OnEnter()
    {
        m_player.m_rotationFollowPlayer = true;
        m_player.m_freezeMovement = false;

        m_player.m_jumpMovement = Vector3.zero;
//        m_player.m_rigidBody.velocity = Vector3.zero;

        m_player.m_enemyDetected = false;

        //EffectsManager.Instance.GetEffect(m_smokeCloud, m_smokePosition);
    }

    public override void OnExit()
    {
        //m_player.m_rotationFollowPlayer = false;
    }
}
