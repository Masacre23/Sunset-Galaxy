using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSpace : MonoBehaviour {

    public List<ZoneSpace> m_previousZones;
	
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = other.GetComponent<Player>();
            if ( !player.m_currentZone || m_previousZones.Contains(player.m_currentZone) || player.m_currentZone == this)
            {
                player.m_currentZone = this;
            }else
            {
                player.m_damageData.m_recive = true;
                player.m_damageData.m_damage = 0;
                player.m_damageData.m_respawn = true;
            }
        }
    }
}
