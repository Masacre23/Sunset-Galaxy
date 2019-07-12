using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    [Header("Character class")]
    public float m_maxHealth = 120.0f;
    public float m_health = 120.0f;
    public float m_moveSpeed = 4.0f;
    float m_turnSpeed;
    public float m_jumpForceVertical = 4.0f;
    public float m_lerpSpeed = 10.0f;

    [HideInInspector] public Animator m_animator;
    [HideInInspector] public GameObjectGravity m_gravityOnCharacter;
    [HideInInspector] public Rigidbody m_rigidBody;
    [HideInInspector] public CapsuleCollider m_capsule;
    [HideInInspector] public float m_capsuleHeight;

    protected float m_groundCheckDistance;
    protected float m_defaultGroundCheckDistance = 0.30f;

    protected bool m_isJumping = false;

    protected bool m_isGrounded;

    public virtual void Awake() {
        if (!(m_gravityOnCharacter = GetComponent<GameObjectGravity>()))
            m_gravityOnCharacter = gameObject.AddComponent<GameObjectGravity>();

        if (tag == "Player")
            m_animator = GetComponent<Animator>();
        else
            m_animator = transform.GetChild(0).GetComponent<Animator>();
    }

    // Use this for initialization
    public virtual void Start() {
        m_rigidBody = GetComponent<Rigidbody>();
        if (tag == "Player") {
            m_capsule = GetComponent<CapsuleCollider>();
            m_capsuleHeight = m_capsule.height;
        }

        m_rigidBody.freezeRotation = true;
        m_health = m_maxHealth;
    }

    public virtual void Restart() {
        m_rigidBody.velocity = Vector3.zero;
        //m_gravityOnCharacter.ReturnToPlanet();
    }

    public virtual void Update() {
    }

    public virtual void LateUpdate() {
    }

    // We use FixedUpdate since we will be dealing with forces
    // This method should control character's movements
    public virtual void FixedUpdate() {
    }

    // This function checks if the character is currently touching a collider below their "feet"
    public bool CheckGroundStatus() {
        RaycastHit hitInfo = new RaycastHit();
        Debug.DrawLine(transform.position + (transform.up * 0.1f), transform.position + (transform.up * 0.1f) + (-transform.up * m_groundCheckDistance), Color.magenta);
        if (GroundCheck(ref hitInfo)) {
            if (this is Player)
                ((Player)this).m_tagGround = hitInfo.collider.tag;
            m_gravityOnCharacter.GravityOnFeet(hitInfo);
            m_isGrounded = true;
            m_isJumping = false;
            m_gravityOnCharacter.m_getStrongestGravity = true;
        } else {
            m_isGrounded = false;
            m_gravityOnCharacter.m_getStrongestGravity = false;
        }

        return m_isGrounded;
    }

    //This function rotates the character so its Vector.up aligns with the direction of the attractor's gravity
    public void UpdateUp() {
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, m_gravityOnCharacter.m_gravity) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, m_lerpSpeed * Time.fixedDeltaTime);
    }

    //This function deals with the jump of the character
    //It mainly adds a velocity to the rigidbody in the direction of the gravity.
    public virtual void Jump(float inputHorizontal, float inputVertical) {
        Vector3 gravity = m_gravityOnCharacter.m_gravity;
        float fallVelocity = Vector3.Dot(gravity, m_rigidBody.velocity);
        m_rigidBody.velocity += gravity * (m_jumpForceVertical - fallVelocity);
        m_isGrounded = false;
        m_isJumping = true;
        m_groundCheckDistance = 0.01f;
    }

    //This function should be called while character is on air.
    //It controls the detection of the floor. If the character is going up, the detection is small in order to avoid being unable to jump.
    public void OnAir() {
        if (Vector3.Dot(m_rigidBody.velocity, transform.up) < 0)
            m_groundCheckDistance = m_defaultGroundCheckDistance;
        else
            m_groundCheckDistance = 0.01f;
    }

    protected bool GroundCheck(ref RaycastHit hitInfo, bool withSphereCast = false) {
        bool ret = false;
        int ignoreWater = 1 << LayerMask.NameToLayer("Water");
        ignoreWater = ignoreWater | 1 << LayerMask.NameToLayer("GeneralTrigger");
        ignoreWater = ignoreWater | 1 << LayerMask.NameToLayer("StencilDetector");
        ignoreWater = ~ignoreWater;

        ret = Physics.Raycast(transform.position + (transform.up * 0.1f), -transform.up, out hitInfo, m_groundCheckDistance, ignoreWater);
        if (!ret && withSphereCast)
            ret = Physics.SphereCast(transform.position + (transform.up * 0.1f), m_capsule.radius, -transform.up, out hitInfo, m_groundCheckDistance, ignoreWater);

        return ret;
    }
}