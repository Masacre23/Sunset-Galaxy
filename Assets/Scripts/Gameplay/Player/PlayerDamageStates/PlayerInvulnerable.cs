using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvulnerable : PlayerDamageStates
{
    private float m_notReciveTime = 2.0f;
    private float m_speedPaint = 0.1f;
    private float m_currentTime;
    SkinnedMeshRenderer[] meshes;

    public override void Start()
    {
        base.Start();
        m_type = States.INVULNERABLE;

        m_currentTime = 0.0f;
        meshes = m_player.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    public override bool OnUpdate(DamageData data)
    {
        bool ret = false;

        m_currentTime += Time.deltaTime;

        float aWithDecimal = m_currentTime / m_speedPaint;
        int aWithoutDecimal = (int)aWithDecimal;
        float a = aWithDecimal - aWithoutDecimal;
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].enabled = (a > 0.5);
        }

        if (m_currentTime >= m_notReciveTime)
        {
            m_player.m_playerDamageState = m_player.m_vulnerable;
            ret = true;
        }

        if (data.m_recive && data.m_respawn)
        {
            m_player.ChangeStateOnDamage();
            //m_player.m_health -= data.m_damage;
            //HUDManager.LostLife();
            if (m_player.m_health <= 0)
                m_player.m_playerDamageState = m_player.m_deadState;
            else
            {
                //m_player.m_pickedObject.Drop();
                m_player.m_playerRespawn.ReSpawn(m_player.m_checkPoint);
                m_player.m_playerDamageState = m_player.m_invulnerable;

                m_player.m_negatePlayerInput = true;
               // m_player.m_soundEffects = null;
            }
            ret = true;
        }

        return ret;
    }

    public override void OnEnter(DamageData data)
    {
        m_currentTime = 0.0f;
    }

    public override void OnExit(DamageData data)
    {
        for (int i = 0; i < meshes.Length; i++)
            meshes[i].enabled = true;
    }
}
