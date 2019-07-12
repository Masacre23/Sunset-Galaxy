using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class VariableCam : MonoBehaviour
{
    public float m_turnSpeed = 1.5f;
    public float m_returnSpeed = 1f;

    public bool m_autoReturnCam = false;
    public float m_maxReturnTime = 1f;

    public Ray m_camRay;

    public Player m_player;
    public Transform m_followingPoint;
    public Transform m_model;
    public Transform m_pivot;
    public Transform m_cam;

    public CameraStates m_currentState;
    [HideInInspector] public CameraStates m_onBack;
    [HideInInspector] public CameraStates m_transit;
    [HideInInspector] public CameraStates m_onFixedPoint;

    [HideInInspector] public VariableCameraProtectFromWallClip m_cameraProtection;

    public CameraPlayerDetector m_playerInnerDetector;
    public CameraPlayerDetector m_playerOuterDetector;

    public float m_followingMaxSpeed = 1f;
    public float m_followingBeginTime = 1.0f;
    public AnimationCurve m_followingBeginSpeed;
    float m_followingTime = 0.0f;

    bool m_followPlayer = true;

    public enum FollowingPlayerState
    {
        STARTING,
        FOLLOWING,
        OFF,
        AUTO
    }

    public FollowingPlayerState m_followingState = FollowingPlayerState.OFF;

    void Awake()
    {
        m_onBack = GetComponent<CameraOnBack>();
        if (!m_onBack)
            m_onBack = gameObject.AddComponent<CameraOnBack>();
        m_onFixedPoint = GetComponent<CameraFixed>();
        if (!m_onFixedPoint)
            m_onFixedPoint = gameObject.AddComponent<CameraFixed>();
        m_transit = GetComponent<CameraTransiting>();
        if (!m_transit)
            m_transit = gameObject.AddComponent<CameraTransiting>();

        m_currentState = m_onBack;
        m_cam.localPosition = ((CameraOnBack)m_onBack).m_camPosition;
    }

    // Use this for initialization
    void Start()
    {
        m_cameraProtection = GetComponent<VariableCameraProtectFromWallClip>();

        m_camRay = new Ray(m_cam.transform.position, m_cam.transform.forward);
    }

    public void OnUpdate(float axisX, float axisY, bool moveCamBehind, bool rotateOnPlayer, float deltaTime)
    {
        FollowingTarget(deltaTime);

        CameraStates previousState = m_currentState;
        if (m_currentState.OnUpdate(axisX, axisY, moveCamBehind, deltaTime))
        {
            previousState.OnExit();
            m_currentState.OnEnter();
        }

        m_camRay.origin = m_cam.transform.position;
        m_camRay.direction = m_cam.transform.forward;

        if (rotateOnPlayer)
        {
            RotateOnTarget(deltaTime);
        }
    }

    private void FollowingTarget(float deltaTime)
    {
        switch (m_followingState)
        {
            case FollowingPlayerState.STARTING:
                {
                    m_followingTime += deltaTime;
                    float perc = m_followingTime / m_followingBeginTime;
                    if (perc > 1.0f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, m_followingPoint.position, deltaTime * m_followingMaxSpeed);
                        m_followingState = FollowingPlayerState.FOLLOWING;
                    }
                    else
                    {
                        float currentSpeed = m_followingBeginSpeed.Evaluate(perc) * m_followingMaxSpeed;
                        transform.position = Vector3.MoveTowards(transform.position, m_followingPoint.position, deltaTime * currentSpeed);
                    }

                    if (m_playerInnerDetector.m_playerInside && CameraHasReachedPlayer())
                    {
                        m_followingState = FollowingPlayerState.OFF;
                    }
                    break;
                }
            case FollowingPlayerState.FOLLOWING:
                {
                    transform.position = Vector3.Lerp(transform.position, m_followingPoint.position, deltaTime * m_followingMaxSpeed);
                    if (m_playerInnerDetector.m_playerInside && CameraHasReachedPlayer())
                    {
                        m_followingState = FollowingPlayerState.OFF;
                    }
                    break;
                }
            case FollowingPlayerState.OFF:
                {
                    if (!m_playerOuterDetector.m_playerInside && m_followPlayer)
                    {
                        m_followingTime = 0.0f;
                        m_followingState = FollowingPlayerState.STARTING;
                    }
                }
                break;
        }
    }

    private bool CameraHasReachedPlayer()
    {
        return Vector3.SqrMagnitude(transform.position - m_followingPoint.position) < 0.01;
    }

    public void CameraFollowingOff()
    {
        m_followingState = FollowingPlayerState.OFF;
        m_followPlayer = false;
    }

    public void SetCamOnPlayer()
    {
        transform.position = m_followingPoint.position;
        transform.rotation = m_followingPoint.rotation;
        if (m_currentState != m_onBack)
        {
            m_currentState.OnExit();
            m_currentState = m_onBack;
            m_currentState.OnEnter();
        }
        ((CameraOnBack)m_onBack).MoveCamBehind();
        m_followPlayer = true;
    }

    public void RotateOnTarget(float deltaTime)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, m_player.transform.rotation, deltaTime * m_returnSpeed);
    } 

    public void SetCameraOnBackLookingAtTarget(Transform orientation, float transitionTime = 0.0f)
    {
        CameraTransiting transitingCam = (CameraTransiting)m_transit;
        transitingCam.ResetTime();

        Vector3 pivotForward = transform.InverseTransformDirection(orientation.forward);
        Vector3 pivotUp = transform.InverseTransformDirection(orientation.up);
        Quaternion lookRotation = Quaternion.LookRotation(pivotForward, pivotUp);

        Quaternion finalRotation = Quaternion.Euler(0.0f, lookRotation.eulerAngles.y, 0.0f) * Quaternion.Euler(m_pivot.transform.localRotation.eulerAngles.x, 0.0f, 0.0f);

        CameraOnBack onBack = (CameraOnBack)m_onBack;
        transitingCam.SetTransitionValues(m_onBack, onBack.m_camPosition, onBack.m_pivotPosition, Quaternion.identity, finalRotation, true, transitionTime);
        m_currentState.EnableCameraChange();
    }

    //This function creates a transition from current camera to onBack camera
    public void SetCameraOnBack(float transitionTime = 0.0f)
    {
        CameraTransiting transitingCam = (CameraTransiting)m_transit;
        transitingCam.ResetTime();

        CameraOnBack onBack = (CameraOnBack)m_onBack;
        transitingCam.SetTransitionValues(m_onBack, onBack.m_camPosition, onBack.m_pivotPosition, Quaternion.identity, onBack.m_savedPivotQuaternion, true, transitionTime);
        m_currentState.EnableCameraChange();
    }

    //This function creates a transition from current camera to Fixed camera
    public void SetCameraOnFixed(Transform camPosition, float transitionTime = 0.0f)
    {
        CameraTransiting transitingCam = (CameraTransiting)m_transit;
        transitingCam.ResetTime();

        CameraFixed fixedCam = (CameraFixed)m_onFixedPoint;

        transitingCam.SetTransitionValues(m_onFixedPoint, Vector3.zero, camPosition.position, Quaternion.identity, camPosition.rotation, false, transitionTime);
        fixedCam.SetTransform(camPosition.position, camPosition.rotation);
        m_currentState.EnableCameraChange();
    }

}
