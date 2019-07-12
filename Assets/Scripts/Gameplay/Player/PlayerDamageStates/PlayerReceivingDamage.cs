using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReceivingDamage : PlayerDamageStates
{
    string m_animationName = "Hit";

    public override void Start()
    {
        base.Start();
        m_type = States.RECEIVING;
    }

    public override bool OnUpdate(DamageData data)
    {
        bool ret = false;

        AnimatorStateInfo animatorInfo = m_player.m_animator.GetCurrentAnimatorStateInfo(0);
        if (!animatorInfo.IsName(m_animationName))
        {
            ret = true;
            m_player.m_playerDamageState = m_player.m_invulnerable;
        }

        return ret;
    }

    public override void OnEnter(DamageData data)
    {
        //m_player.m_pickedObject.Drop();
        m_player.ChangeStateOnDamage();
        m_player.m_negatePlayerInput = true;
        m_player.m_animator.SetBool("Damaged", true);

        m_player.m_damageForceToApply = true;
        m_player.m_damageForce = data.m_force;
    }

    public override void OnExit(DamageData data)
    {
        m_player.m_negatePlayerInput = false;
        m_player.m_animator.SetBool("Damaged", false);
    }
}
