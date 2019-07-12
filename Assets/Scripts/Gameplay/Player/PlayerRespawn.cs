using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawn : MonoBehaviour
{
	public GameObject[] spawnPoints;
	float fadeTime = 1.0f;
	public Image fadePanel;
	GameObject player;

    int m_playerLayer;
    int m_terrainLayer;
    int m_floorLayer;
    int m_watterLayer;

    void Start()
	{
		player = GameObject.Find ("Player");

        m_playerLayer = LayerMask.NameToLayer("Player");
        m_terrainLayer = LayerMask.NameToLayer("Terrain");
        m_floorLayer = LayerMask.NameToLayer("Floor");
        m_watterLayer = LayerMask.NameToLayer("HarmfulTerrain");
    }

	IEnumerator Fading(Transform spawn)
	{
		for (float t = 0.0f; t < fadeTime;) 
		{
			t += Time.deltaTime ;
			fadePanel.color = new Color (0f, 0f, 0f, t / (fadeTime));
			yield return null;
		}
        player.transform.position = spawn.position;
		player.transform.rotation = spawn.rotation;
		player.transform.GetChild(0).transform.rotation = spawn.rotation;
        player.GetComponent<Player>().Restart();
        player.GetComponent<Rigidbody>().ResetInertiaTensor();
        
        Physics.IgnoreLayerCollision(m_playerLayer, m_terrainLayer, false);
        Physics.IgnoreLayerCollision(m_playerLayer, m_floorLayer, false);
        Physics.IgnoreLayerCollision(m_playerLayer, m_watterLayer, false);

        for (float t = fadeTime; t > 0.0f;) 
		{
			t -= Time.deltaTime;
			fadePanel.color = new Color (0f, 0f, 0f, t / (fadeTime));
			yield return null;
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.name == "Player")
			StartCoroutine (Fading (spawnPoints[0].transform));
	}

    public void ReSpawn(Transform spawn)
    {
        StartCoroutine(Fading(spawn));
    }
}
