using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVulnerable : PlayerDamageStates
{

    public override void Start()
    {
        base.Start();
        m_type = States.VULNERABLE;
    }

    public override bool OnUpdate(DamageData data)
    {
        bool ret = false;

        if (data.m_recive)
        {
            ret = true;
            m_player.m_health -= data.m_damage;
            //if(data.m_damage > 0)
               // HUDManager.LostLife();
            if (m_player.m_health <= 0)
                m_player.m_playerDamageState = m_player.m_deadState;
            else
            {
                if (data.m_respawn)
                {
                    //m_player.m_pickedObject.Drop();
                    m_player.m_playerRespawn.ReSpawn(m_player.m_checkPoint);
                    m_player.m_playerDamageState = m_player.m_invulnerable;

                    m_player.m_negatePlayerInput = true;
                   // m_player.m_soundEffects = null;
                }
                else
                    m_player.m_playerDamageState = m_player.m_receivingDamage;
            }
        }

        return ret;
    }

    public override void OnEnter(DamageData data)
    {
    }

    public override void OnExit(DamageData data)
    {
    }
}
