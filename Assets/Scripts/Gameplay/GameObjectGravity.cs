using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class should be added to any GameObject which gravity can be changed during the game.
//It controls the current gravity of the object, and adds it to its rigid body.
public class GameObjectGravity : MonoBehaviour {
    [HideInInspector] public Rigidbody m_rigidBody;
    public GameObject m_attractorGameObject;
    public Vector3 m_gravity { get; private set; }

    [SerializeField] List<GravityAttractor> m_planetsGravity;
    [SerializeField] List<GravityAttractor> m_objectsGravity;

    Dictionary<GameObject, List<GravityAttractor>> m_planetsGravityByAttractor;
    Dictionary<GameObject, List<GravityAttractor>> m_objectsGravityByAttractor;


    public float m_maxTimeTravelled = 0.5f;
    public float m_maxTimeOnGravityWallGravity = 1.0f;

    public bool m_ignoreGravity = false;
    float m_timeTravelled;
    Vector3 m_impulseForce;

    public bool m_intoWater = false;

    public bool m_getStrongestGravity = false;

    //This should be the same for all gameobjects
    static float m_gravityStrength = -19.0f;
    static float m_waterResistance = 19.0f;

    private bool m_inGravityWall = false;
    private Vector3 m_savedGravity = Vector3.zero;

   // Player m_player;

    void Awake() {
        m_planetsGravity = new List<GravityAttractor>();
        m_objectsGravity = new List<GravityAttractor>();

        m_planetsGravityByAttractor = new Dictionary<GameObject, List<GravityAttractor>>();
        m_objectsGravityByAttractor = new Dictionary<GameObject, List<GravityAttractor>>();


        m_rigidBody = GetComponent<Rigidbody>();
        m_rigidBody.useGravity = false;
        m_rigidBody.velocity = Vector3.zero;

        m_gravity = Vector3.zero;
    }

    // Use this for initialization
    void Start() {
        m_impulseForce = Vector3.zero;
        m_attractorGameObject = null;

      //  m_player = GetComponent<Player>();
    }

    // Called to add gravity force into the rigid body.
    public void FixedUpdate() {
        if (!m_ignoreGravity) {
            if (!m_inGravityWall)
                m_gravity = GetGravity();
            else
                m_gravity = m_savedGravity;

            float strength = m_gravityStrength;
            if (m_intoWater) {
                strength += m_waterResistance;
            }
            m_rigidBody.AddForce(strength * m_rigidBody.mass * m_gravity);
        } else {
            m_timeTravelled += Time.fixedDeltaTime;
            if (m_timeTravelled > m_maxTimeTravelled) {
                m_timeTravelled = 0.0f;
                m_ignoreGravity = false;
            }
        }
    }

    // Compute the gravity of the object
    private Vector3 GetGravity() {
        Vector3 gravity = Vector3.zero;

        if (m_objectsGravity.Count > 0) {
            if (m_getStrongestGravity) {
                foreach (GravityAttractor attractor in m_objectsGravity) {
                    if (attractor.m_attractor == m_attractorGameObject) {
                        gravity += attractor.GetGravityWithDistance(transform.position);
                    }
                }
            } else if (GetNumAttractors() == 1) {
                foreach (GravityAttractor attractor in m_objectsGravity) {
                    gravity += attractor.GetGravityWithDistance(transform.position);
                }
            } else {
                foreach (GravityAttractor attractor in m_objectsGravity) {
                    Vector3 newGravity = Vector3.zero;
                    float newDistance = 0.0f;
                    attractor.GetDistanceAndGravityVector(transform.position, ref newGravity, ref newDistance);

                    float intensity = 1.0f / newDistance;
                    if (intensity > 1000.0f)
                        intensity = 1000.0f;

                    gravity += newGravity * intensity;
                }
            }
        } else {
            foreach (GravityAttractor planet in m_planetsGravity) {
                gravity += planet.GetGravityWithDistance(transform.position);
            }
        }

        //print(gravity);

        return gravity.normalized;
    }

    // Detects the strongest gravity attractor (the one the player is closest to)
    private GameObject GetStrongestAttractor() {
        GameObject objectAttractor = null;
        float distance = 1000.0f;

        foreach (GravityAttractor attractor in m_objectsGravity) {
            float attractorDistance = attractor.GetDistance(transform.position);
            if (attractorDistance < distance)
                objectAttractor = attractor.m_attractor;
        }

        return objectAttractor;
    }

    // Returns the number of attractors the object has
    private int GetNumAttractors() {
        return m_objectsGravityByAttractor.Count;
    }

    //This function sets the passed raycasthit as the current attractor for the game object
    //It's mainly to be used by characters in order update its attractor while they walk on an attractor, not on planet
    public void GravityOnFeet(RaycastHit hit) {
        AttractorProperties attractorProperties = hit.transform.gameObject.GetComponent<AttractorProperties>();
        if (attractorProperties)
            m_attractorGameObject = attractorProperties.m_attractor;

        if (hit.collider.tag == "GravityWall")
            m_savedGravity = hit.normal;
    }

    public void EnterGravityWallZone() {
        m_inGravityWall = true;
        m_savedGravity = m_gravity;
    }

    public void ExitGravityWallZone() {
        m_inGravityWall = false;
    }

    //Call this function when adding a new attractor to the object
    public void AddAttractor(GravityAttractor newAttractor) {
        if (newAttractor.m_type == GravityAttractor.GravityType.PLANET) {
            if (!m_planetsGravity.Contains(newAttractor)) {
                m_planetsGravity.Add(newAttractor);
                AddingToDictionary(m_planetsGravityByAttractor, newAttractor);
            }
        } else {
            if (!m_objectsGravity.Contains(newAttractor)) {
                m_objectsGravity.Add(newAttractor);
                AddingToDictionary(m_objectsGravityByAttractor, newAttractor);
            }
        }
    }

    //Call this function when removing an attractor for the object
    public void RemoveAttractor(GravityAttractor oldAttractor) {
        if (m_planetsGravity.Contains(oldAttractor)) {
            m_planetsGravity.Remove(oldAttractor);
            RemovingFromDictionary(m_planetsGravityByAttractor, oldAttractor);
        } else if (m_objectsGravity.Contains(oldAttractor)) {
            m_objectsGravity.Remove(oldAttractor);
            RemovingFromDictionary(m_objectsGravityByAttractor, oldAttractor);
        }
    }

    private void AddingToDictionary(Dictionary<GameObject, List<GravityAttractor>> dictionary, GravityAttractor newAttractor) {
        List<GravityAttractor> newList;
        if (dictionary.ContainsKey(newAttractor.m_attractor)) {
            dictionary.TryGetValue(newAttractor.m_attractor, out newList);
            newList.Add(newAttractor);
        } else {
            newList = new List<GravityAttractor>();
            newList.Add(newAttractor);
            dictionary.Add(newAttractor.m_attractor, newList);
        }
    }

    private void RemovingFromDictionary(Dictionary<GameObject, List<GravityAttractor>> dictionary, GravityAttractor oldAttractor) {
        List<GravityAttractor> newList;
        if (dictionary.ContainsKey(oldAttractor.m_attractor)) {
            dictionary.TryGetValue(oldAttractor.m_attractor, out newList);
            newList.Remove(oldAttractor);
            if (newList.Count == 0) {
                dictionary.Remove(oldAttractor.m_attractor);
            }
        }
    }


}
