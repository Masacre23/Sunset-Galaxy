using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOnBack : CameraStates
{
    float m_returnTime = 0.0f;
    float m_lookAngle = 0.0f;
    float m_tiltAngle = 0.0f;

    public float m_defaultTiltAngle = 45f;
    public float m_tiltMax = 60;                       // The maximum value of the x axis rotation of the pivot.
    public float m_tiltMin = 45f;
    public Vector3 m_camPosition = new Vector3(0.0f, 2.0f, -5.0f);
    [HideInInspector] public Vector3 m_pivotPosition { get; private set; }
    public float m_transitionToBackTime = 0.5f;

    [HideInInspector] public Quaternion m_savedPivotQuaternion = Quaternion.identity;

    public override void Start()
    {
        base.Start();
        m_type = States.BACK;
        m_tiltAngle = m_defaultTiltAngle;

        m_pivotPosition = Vector3.zero;
    }

    //Main camera update. Returns true if a change in state ocurred (in order to call OnExit() and OnEnter())
    public override bool OnUpdate(float axisHorizontal, float axisVertical, bool returnCam, float timeStep)
    {
        bool ret = false;  

        CameraRotation(axisHorizontal, axisVertical, timeStep);

        if (m_changeCamState)
        {
            //m_savedPivotQuaternion = m_variableCam.m_pivot.localRotation;
            m_savedPivotQuaternion = Quaternion.Euler(m_defaultTiltAngle, m_variableCam.m_model.localRotation.eulerAngles.y, 0);
            m_variableCam.m_currentState = m_variableCam.m_transit;
            ret = true;
        }
        else if (returnCam)
        {
            m_savedPivotQuaternion = Quaternion.Euler(m_defaultTiltAngle, m_variableCam.m_model.localRotation.eulerAngles.y, 0);
            m_variableCam.SetCameraOnBack(m_transitionToBackTime);
            m_variableCam.m_currentState = m_variableCam.m_transit;
            ret = true;
        }
        else if (m_variableCam.m_autoReturnCam)
        {
            if (m_variableCam.m_player.m_playerStopped && axisHorizontal == 0 && axisVertical == 0)
            {
                m_returnTime += timeStep;
                if (m_returnTime > m_variableCam.m_maxReturnTime)
                {
                    m_savedPivotQuaternion = Quaternion.Euler(m_defaultTiltAngle, m_variableCam.m_model.localRotation.eulerAngles.y, 0);
                    m_variableCam.SetCameraOnBack(m_transitionToBackTime);
                    m_variableCam.m_currentState = m_variableCam.m_transit;
                    ret = true;
                }
            }
            else
                m_returnTime = 0.0f;
        }

        return ret;
    }

    public override void OnEnter()
    {
        m_returnTime = m_variableCam.m_maxReturnTime;
        m_changeCamState = false;
        //m_variableCam.m_cameraProtection.SetProtection(true);
        //m_lookAngle = m_savedPivotQuaternion.eulerAngles.y;
        //m_tiltAngle = m_defaultTiltAngle;
        Vector3 eulers = m_variableCam.m_pivot.localRotation.eulerAngles;
        m_lookAngle = eulers.y;
        m_tiltAngle = eulers.x;
    }

    public override void OnExit()
    {
        m_returnTime = 0.0f;
        m_changeCamState = false;
        //m_variableCam.m_cameraProtection.SetProtection(false);
        //m_lookAngle = m_variableCam.m_model.localRotation.eulerAngles.y;
        //m_tiltAngle = m_defaultTiltAngle;
    }

    void CameraRotation(float x, float y, float deltaTime)
    {
        m_lookAngle += x * m_variableCam.m_turnSpeed;
        m_tiltAngle -= y * m_variableCam.m_turnSpeed;
        m_tiltAngle = Mathf.Clamp(m_tiltAngle, m_tiltMin, m_tiltMax);

        Quaternion targetRotation = Quaternion.Euler(m_lookAngle * Vector3.up);
        Quaternion tiltRotation = Quaternion.Euler(m_tiltAngle, 0.0f, 0.0f);

        m_variableCam.m_pivot.localRotation = targetRotation * tiltRotation;
    }

    public void MoveCamBehind()
    {
        m_lookAngle = m_variableCam.m_model.localRotation.eulerAngles.y;
        m_tiltAngle = m_defaultTiltAngle;

        Quaternion targetRotation = Quaternion.Euler(m_lookAngle * Vector3.up);
        Quaternion tiltRotation = Quaternion.Euler(m_tiltAngle, 0.0f, 0.0f);

        m_variableCam.m_pivot.localRotation = targetRotation * tiltRotation;
    }
}
