using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDead : PlayerDamageStates
{
    string m_animationName = "Dead";

    public override void Start()
    {
        base.Start();
        m_type = States.DEAD;
    }

    public override bool OnUpdate(DamageData data)
    {
        bool ret = false;

        AnimatorStateInfo animatorInfo = m_player.m_animator.GetCurrentAnimatorStateInfo(0);
        if (animatorInfo.IsName(m_animationName) && animatorInfo.normalizedTime >= 1.0f)
        {
            OnExit(data);
        }

        return ret;
    }

    public override void OnEnter(DamageData data)
    {
        //m_player.m_pickedObject.Drop();
        m_player.ChangeStateOnDamage();
        m_player.m_negatePlayerInput = true;
        m_player.m_animator.SetBool("Dead", true);

        /*if (m_player.m_soundEffects)
        {
            m_player.m_soundEffects.PlayGameOver();
        }*/
            
        if (data.m_respawn)
            OnExit(data);
    }

    public override void OnExit(DamageData data)
    {
        //Scenes.LoadScene(Scenes.GameOver);
        //HUDManager.ShowGameOverPanel(true);
        m_player.m_animator.SetBool("Dead", false);
    }
}
