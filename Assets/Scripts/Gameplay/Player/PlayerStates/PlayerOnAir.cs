using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnAir : PlayerStates
{
    public float m_oxigenDoubleJump = 20.0f;
    private bool m_doubleJump = false;

    public override void Start()
    {
        base.Start();
        m_type = States.ONAIR;
    }

    //Main player update. Returns true if a change in state ocurred (in order to call OnExit() and OnEnter())
    public override bool OnUpdate(float axisHorizontal, float axisVertical, bool jumping, bool pickObjects, bool aimingObject, float timeStep)
    {
        bool ret = false;

        m_player.CheckPlayerStopped(axisHorizontal, axisVertical);

        m_player.OnAir();
        m_player.UpdateUp();
        m_player.MoveOnAir(timeStep);

        if (m_player.CheckGroundAndEnemyStatus())
        {
            /*if (m_player.m_pickedObject.HasObjectsToThrow())
                m_player.m_currentState = m_player.m_carrying;
            else*/
                m_player.m_currentState = m_player.m_grounded;

            ret = true;
        }

        return ret;
    }

    public override void OnEnter()
    {
        m_doubleJump = false;
    }

    public override void OnExit()
    {
    }
}
