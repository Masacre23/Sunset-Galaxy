using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the main class for the player. It will control the player input and character movement.
//It inherits from Character.
public class Player : Character {
    [Header("Movement factors")]
    public float m_speedFactor = 2.0f;
    public float m_carryingFactor = 1.0f;

    //Variables regarding player input control
    PlayerController m_playerInput;
    float m_axisHorizontal;
    float m_axisVertical;
    float m_camHorizontal;
    float m_camVertical;
    public bool m_jumping;
    bool m_pickObjects;
    bool m_aimObject;
    bool m_returnCam;

    [HideInInspector] public bool m_throwAnimation = false;

    [Header("Input bools")]
    public bool m_negatePlayerInput = false;
    public bool m_savedPlayerInput = false;
    public bool m_paused = false;
    public bool m_justUnpaused = false;

    //Variables regarding player state
    [Header("Player States")]
    public PlayerStates m_currentState;
    [HideInInspector] public PlayerStates m_grounded;
    [HideInInspector] public PlayerStates m_onAir;
    [HideInInspector] public PlayerCarrying m_carrying;

    //Variables regarding player damage state
    public PlayerDamageStates m_playerDamageState;
    [HideInInspector] public PlayerDamageStates m_vulnerable;
    [HideInInspector] public PlayerDamageStates m_invulnerable;
    [HideInInspector] public PlayerDamageStates m_receivingDamage;
    [HideInInspector] public PlayerDamageStates m_deadState;
    [HideInInspector] public DamageData m_damageData;
    [HideInInspector] public PlayerRespawn m_playerRespawn;
    public Transform m_checkPoint;

    [HideInInspector] public bool m_damageForceToApply = false;
    [HideInInspector] public Vector3 m_damageForce;

    //General Info variables
    [HideInInspector] public Transform m_modelTransform;
    [HideInInspector] public VariableCam m_camController;

    //Variables regarding player movement
    [Header("Movement")]
    public ZoneSpace m_currentZone = null;
    public bool m_freezeMovement;
    public bool m_rotationFollowPlayer;
    [HideInInspector] public bool m_playerStopped = false;
    public Vector3 m_offset = Vector3.zero;
    public Vector3 m_lastMovement = Vector3.zero;
    public string m_tagGround = "";
    public float m_timeSlide = 0.2f;
    public float m_slideSpeed = 2.0f;
    private Vector3 m_rigidBodyTotal = Vector3.zero;
    [HideInInspector] public bool m_doubleJumping = false;
    [HideInInspector] public Vector3 m_jumpMovement;
    private bool m_hasJumped = false;
    private Vector3 m_jumpVector;

    public bool m_jumpOnEnemy { get; private set; }
    [HideInInspector] public bool m_enemyDetected = false;
    [HideInInspector] public GameObject m_lastEnemyJumped = null;

    //Variables regarding player's change of gravity
    [HideInInspector] public TargetDetectorByTag m_gravityTargets;

    //Variables regarding player's throw of objects
    [Header("Throw variables")]
    public float m_throwDetectionRange = 20.0f;
    public float m_throwForce = 2.0f;
    public float m_angleEnemyDetection = 30.0f;

    //Variables regarding player picking up objects
    //[HideInInspector] public PickedObject m_pickedObject;

    //Effects
    [Header("Others")]
    //[HideInInspector] public EnemyDetectorByLayer m_enemyDetector;
    [HideInInspector] public float m_inputSpeed;
    private float m_runSpeed;
    private float m_carryingSpeed;

    //public PlayerSoundEffects m_soundEffects;

    public override void Awake() {
        m_grounded = gameObject.GetComponent<PlayerGrounded>();
        if (!m_grounded)
            m_grounded = gameObject.AddComponent<PlayerGrounded>();
        m_onAir = gameObject.GetComponent<PlayerOnAir>();
        if (!m_onAir)
            m_onAir = gameObject.AddComponent<PlayerOnAir>();
        m_carrying = gameObject.GetComponent<PlayerCarrying>();
        if (!m_carrying)
            m_carrying = gameObject.AddComponent<PlayerCarrying>();

        m_currentState = m_onAir;

        m_vulnerable = gameObject.GetComponent<PlayerVulnerable>();
        if (!m_vulnerable)
            m_vulnerable = gameObject.AddComponent<PlayerVulnerable>();
        m_invulnerable = gameObject.GetComponent<PlayerInvulnerable>();
        if (!m_invulnerable)
            m_invulnerable = gameObject.AddComponent<PlayerInvulnerable>();
        m_receivingDamage = gameObject.GetComponent<PlayerReceivingDamage>();
        if (!m_receivingDamage)
            m_receivingDamage = gameObject.AddComponent<PlayerReceivingDamage>();
        m_deadState = gameObject.GetComponent<PlayerDead>();
        if (!m_deadState)
            m_deadState = gameObject.AddComponent<PlayerDead>();
        m_playerDamageState = m_vulnerable;
        m_damageData = new DamageData();
        m_playerRespawn = gameObject.GetComponent<PlayerRespawn>();

        if (!(m_playerInput = GetComponent<PlayerController>()))
            m_playerInput = gameObject.AddComponent<PlayerController>();

        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

        GameObject cameraFree = GameObject.Find("MainCameraRig");
        if (cameraFree)
            m_camController = cameraFree.GetComponent<VariableCam>();
        m_rotationFollowPlayer = true;

        GameObject detectorsEmpty = GameObject.Find("TargetDetectors");
        if (!detectorsEmpty) {
            detectorsEmpty = new GameObject("TargetDetectors");
            detectorsEmpty.transform.parent = transform;
            detectorsEmpty.transform.localPosition = Vector3.zero;
        }

        m_gravityTargets = GetComponentInChildren<TargetDetectorByTag>();

       // m_soundEffects = GetComponent<PlayerSoundEffects>();

       // m_pickedObject = GetComponent<PickedObject>();
       // m_enemyDetector = GetComponentInChildren<EnemyDetectorByLayer>();
        base.Awake();
    }

    // Use this for initialization
    public override void Start() {
        m_modelTransform = transform.Find("Model");
        m_freezeMovement = false;
        m_negatePlayerInput = false;
        base.Start();

        m_runSpeed = m_speedFactor * m_moveSpeed;
        m_carryingSpeed = m_carryingFactor * m_moveSpeed;
        m_rigidBodyTotal = Vector3.zero;

        m_camController.SetCamOnPlayer();

        m_jumpOnEnemy = false;
    }

    public override void Restart() {
        m_currentState.OnExit();
        m_currentState = m_onAir;

        ResetInput();

        m_freezeMovement = false;
        m_negatePlayerInput = false;
        m_currentZone = null;

        m_animator.Rebind();

      //  m_soundEffects = GetComponent<PlayerSoundEffects>();

        m_camController.SetCamOnPlayer();

        base.Restart();
    }

    // This method should control player movements
    // First, it should read input from PlayerController in Update, since we need input every frame
    public override void Update() {
        ManageInput();
        PlayerStates previousState = m_currentState;
        if (m_currentState.OnUpdate(m_axisHorizontal, m_axisVertical, m_jumping, m_pickObjects, m_aimObject, Time.deltaTime)) {
            previousState.OnExit();
            m_currentState.OnEnter();
        }

        m_modelTransform.rotation = Quaternion.FromToRotation(m_modelTransform.up, transform.up) * m_modelTransform.rotation;

        UpdateAnimator();

        this.transform.position = this.transform.position + m_rigidBodyTotal;
        m_rigidBodyTotal = Vector3.zero;

        if (m_hasJumped) {
            m_rigidBody.velocity = m_jumpVector;
            m_hasJumped = false;
        }

        PlayerDamageStates previousDamageState = m_playerDamageState;
        if (m_playerDamageState.OnUpdate(m_damageData)) {
            previousDamageState.OnExit(m_damageData);
            m_playerDamageState.OnEnter(m_damageData);
        }
        m_damageData.ResetDamageData();

        if (m_justUnpaused) {
            m_paused = false;
            m_justUnpaused = false;
        }

        m_doubleJumping = false;
    }

    public override void LateUpdate() {
        if (m_camController) {
            m_camController.OnUpdate(m_camHorizontal, m_camVertical, m_returnCam, m_rotationFollowPlayer, Time.deltaTime);
        }

        ResetInput();
    }

    public void ChangeStateOnDamage() {
        PlayerStates previousState = m_currentState;
        m_currentState = m_onAir;
        previousState.OnExit();
        m_currentState.OnEnter();
    }

    // Second, it should update player state regarding the current state & input
    // We use FixedUpdate when we need to deal with physics
    // We also clean the input only after a FixedUpdate, so we are sure we have at least one FixedUpdate with the correct input recieved in Update
    public override void FixedUpdate() {
        base.FixedUpdate();
        //m_rigidBody.MovePosition(transform.position + m_rigidBodyTotal);
        //m_rigidBodyTotal = Vector3.zero;
        if (m_damageForceToApply) {
            m_rigidBody.AddForce(m_damageForce, ForceMode.VelocityChange);
            m_damageForceToApply = false;
        }
    }

    //This functions computes the direction of the jump from the player input
    //Then calls the jump method in the computed direction
    public override void Jump(float inputHorizontal, float inputVertical) {
        Vector3 movement = Vector3.zero;
        if (m_jumpOnEnemy) {
            m_jumpOnEnemy = false;
            movement = m_modelTransform.forward;
        } else {
            Vector3 forward = GetDirectionForward();
            movement = inputHorizontal * GetDirectionRight() + inputVertical * forward;
            movement.Normalize();
        }

        //float speed = GetSpeedFromInput(m_inputSpeed);
        float speed = m_runSpeed;

        if (movement != Vector3.zero)
            m_modelTransform.rotation = Quaternion.LookRotation(movement, transform.up);

        m_hasJumped = true;
        m_jumpVector = Vector3.zero;
        m_jumpMovement = Vector3.zero;

        m_jumpVector = transform.up * m_jumpForceVertical;
        m_jumpMovement = transform.InverseTransformDirection(movement * speed);
        m_isGrounded = false;
        m_isJumping = true;
        m_groundCheckDistance = 0.01f;
       // if (m_soundEffects != null)
         //   m_soundEffects.PlayJump();
    }

    //This functions controls the character movement and the model orientation.
    public void Move(float timeStep) {
        if (!m_freezeMovement) {
            Vector3 forward = GetDirectionForward();
            Vector3 movement = m_axisHorizontal * GetDirectionRight() + m_axisVertical * forward;
            movement.Normalize();

            float speed = GetSpeedFromInput(m_inputSpeed);

            //m_rigidBody.MovePosition(transform.position + m_offset + movement * speed * timeStep);
            m_rigidBodyTotal += m_offset + movement * speed * timeStep;
            m_offset = Vector3.zero;

            if (movement != Vector3.zero) {
                Quaternion modelRotation = Quaternion.LookRotation(movement, transform.up);
                m_modelTransform.rotation = Quaternion.Slerp(m_modelTransform.rotation, modelRotation, 20.0f * timeStep);
                m_lastMovement = movement;
            }
        }
    }

    //This functions controls the character movement and the model orientation while on air
    public void MoveOnAir(float timeStep) {
        if (!m_freezeMovement) {
            Vector3 forward = GetDirectionForward();
            Vector3 movement = m_axisHorizontal * GetDirectionRight() + m_axisVertical * forward;
            movement.Normalize();

            Vector3 jumpMovement = transform.TransformDirection(m_jumpMovement);
            Vector3 jumpDirection = jumpMovement.normalized;


            //We need to ignore input in the direction of the jump
            Vector3 finalDirection = movement;
            float forwardIntensity = Vector3.Dot(movement, jumpDirection);
            if (forwardIntensity > 0.0f)
                finalDirection -= Vector3.Dot(movement, jumpDirection) * jumpDirection;

            float speed = GetSpeedFromInput(m_inputSpeed);
            Vector3 finalMovement = finalDirection * speed + jumpMovement;
            finalMovement = DetectEnemyBelow(finalMovement);

            m_rigidBodyTotal += m_offset + finalMovement * timeStep;
            m_offset = Vector3.zero;

            if (movement != Vector3.zero) {
                Quaternion modelRotation = Quaternion.LookRotation(movement, transform.up);
                m_modelTransform.rotation = Quaternion.Slerp(m_modelTransform.rotation, modelRotation, 20.0f * timeStep);
                m_lastMovement = movement;
            }
        }
    }

    private Vector3 DetectEnemyBelow(Vector3 movement) {
        Vector3 rayDirection = -5.0f * transform.up + movement;
        rayDirection.Normalize();
        Debug.DrawRay(transform.position, rayDirection, Color.gray);

        RaycastHit hitInfo;
        int enemyLayer = 1 << LayerMask.NameToLayer("Enemy");
        if (Physics.Raycast(transform.position, rayDirection, out hitInfo, 1.0f, enemyLayer)) {
            if (hitInfo.transform.tag == "EnemySnail" && hitInfo.transform.gameObject != m_lastEnemyJumped) {
                m_enemyDetected = true;
                Vector3 toEnemy = hitInfo.transform.position - transform.position;
                toEnemy -= Vector3.Dot(toEnemy, transform.up) * transform.up;
                return (toEnemy + movement) / 2.0f;
            }
        }

        m_enemyDetected = false;
        return movement;
    }

    // This function is similar to Character.CheckGroundStatus, but also checks if the player is falling on an enemy in order to produce a jump
    public bool CheckGroundAndEnemyStatus() {
        RaycastHit hitInfo = new RaycastHit();
        Debug.DrawLine(transform.position + (transform.up * 0.1f), transform.position + (transform.up * 0.1f) + (-transform.up * m_groundCheckDistance), Color.magenta);
        if (GroundCheck(ref hitInfo, m_enemyDetected)) {
            if (hitInfo.transform.tag == "EnemySnail") {
                m_lastEnemyJumped = hitInfo.transform.gameObject;
                m_jumpOnEnemy = true;
              /*  Enemy enemy = hitInfo.collider.gameObject.GetComponent<Enemy>();
                if (enemy != null && !enemy.m_enemyCollider.is_attacking) {
                    enemy.m_sound.PlayCrash();
                    enemy.Stun();
                }*/
            } else {
                m_lastEnemyJumped = null;
               // if (m_soundEffects)
                 //   m_soundEffects.PlayFall();
            }

            m_tagGround = hitInfo.collider.tag;
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

    public void RotateModel(Vector3 forward) {
        Quaternion modelRotation = Quaternion.LookRotation(forward, m_modelTransform.up);
        m_modelTransform.rotation = modelRotation;
    }

    void UpdateAnimator() {
       /* m_animator.SetFloat("HorizontalVelocity", m_inputSpeed);
        m_animator.SetFloat("AirVelocity", Vector3.Dot(m_rigidBody.velocity, transform.up));
        m_animator.SetBool("Grounded", m_isGrounded);
        m_animator.SetBool("Jump", m_isJumping);
        m_animator.SetBool("DoubleJump", m_doubleJumping);
        m_animator.SetBool("Throwing", m_throwAnimation);*/
    }

    public bool TryToPick() {
        /*if (m_pickedObject.CanPickMoreObjects()) {
            if (m_pickedObject.FindObjectToPick(transform.position + transform.up * (m_capsuleHeight / 2))) {
                m_carrying.m_pickingOrThrowing = true;
                return true;
            }
        }*/

        return false;
    }

    public void ThrowObjectsThirdPerson() {
      //  m_pickedObject.SetThrowingForces(m_throwForce);
        m_throwAnimation = true;
    }

    void OnCollisionEnter(Collision col) {
        //m_damage.m_force = -col.relativeVelocity * 10.0f;
        m_damageData.m_force = -col.relativeVelocity.normalized * 2.0f;

        int harmfulTerrain = LayerMask.NameToLayer("HarmfulTerrain");
        if (col.collider.gameObject.layer == harmfulTerrain) {
            m_damageData.m_recive = true;
            m_damageData.m_damage = 20;
            m_damageData.m_respawn = true;
            m_negatePlayerInput = true;
        }

        int enemy = LayerMask.NameToLayer("Enemy");
        if (col.collider.gameObject.layer == enemy) {
            if (col.gameObject.tag == "FlyingEnemy") {
                m_damageData.m_recive = true;
                m_damageData.m_damage = 20;
            }
        }

        int harmfulObject = LayerMask.NameToLayer("HarmfulObject");
        if (col.collider.gameObject.layer == harmfulObject) {
           /* MovingDamagingObject objectMoving = col.collider.GetComponent<MovingDamagingObject>();
            StaticDamageObject objectStatic = col.collider.GetComponent<StaticDamageObject>();
            if (objectMoving) {
                m_damageData.m_recive = true;
                m_damageData.m_damage = objectMoving.m_impactDamage;
                m_damageData.m_force = objectMoving.m_directionMovement * objectMoving.m_forceMultiplier;
            } else if (objectStatic) {
                m_damageData.m_recive = true;
                m_damageData.m_damage = objectStatic.m_impactDamage;
            } else if (col.relativeVelocity.magnitude > 2.0f) {
                m_damageData.m_recive = true;
                m_damageData.m_damage = 20;
            }*/
        }
    }

    private void ManageInput() {
        if (!m_negatePlayerInput && !m_paused) {
            m_playerInput.GetDirections(ref m_axisHorizontal, ref m_axisVertical, ref m_camHorizontal, ref m_camVertical);
            m_playerInput.GetButtons(ref m_jumping, ref m_pickObjects, ref m_aimObject, ref m_returnCam);

        }
        m_playerStopped = false;
        m_inputSpeed = Mathf.Abs(m_axisHorizontal) + Mathf.Abs(m_axisVertical);
    }

    private void ResetInput() {
        m_axisHorizontal = 0.0f;
        m_axisVertical = 0.0f;
        m_camHorizontal = 0.0f;
        m_camVertical = 0.0f;
        m_jumping = false;
        m_pickObjects = false;
        m_aimObject = false;
        m_returnCam = false;
    }

    public void HitDebug() {
        bool hit = Input.GetKeyDown(KeyCode.Alpha8);
        bool dead = Input.GetKeyDown(KeyCode.Alpha9);

        if (hit) {
            m_damageData.m_recive = true;
            m_damageData.m_damage = 20;
            m_damageData.m_force = -m_modelTransform.forward * 2.0f;
        } else if (dead) {
            m_damageData.m_recive = true;
            m_damageData.m_damage = (int)m_health + 1;
        }
    }

    public void PausePlayer() {
        m_paused = true;
        ResetInput();
    }

    public void UnpausePlayer() {
        m_justUnpaused = true;
        ResetInput();
    }

    private Vector3 GetDirectionRight() {
        return Camera.main.transform.right - Vector3.Dot(Camera.main.transform.right, transform.up) * transform.up;
    }

    private Vector3 GetDirectionForward() {
        return Camera.main.transform.forward - Vector3.Dot(Camera.main.transform.forward, transform.up) * transform.up;
    }

    private float GetSpeedFromInput(float inputIntensity) {
        if (m_currentState != m_carrying)
            return inputIntensity > 0.5 ? m_runSpeed : m_moveSpeed;
        else
            return m_carryingSpeed;
    }

    public void CheckPlayerStopped(float axisHorizontal, float axisVertical) {
        if (axisHorizontal == 0.0f && axisVertical == 0.0f)
            m_playerStopped = true;
        else
            m_playerStopped = false;
    }
}