using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransiting : CameraStates {

    public CameraStates m_finalState;
    bool m_transitionStopped = false;

    float m_time;
    float m_addedTime;
    float m_timeBetweenChanges = 0.5f;

    bool m_localPivotTransforms = false;
    bool m_changeWithoutTransition = false;
    Vector3 m_initialCamPosition;
    Vector3 m_targetCamPosition;
    Vector3 m_initialPivotPosition;
    Vector3 m_targetPivotPosition;
    Quaternion m_initialRotationPivot;
    Quaternion m_targetRotationPivot;
    Quaternion m_initialRotationCamera;
    Quaternion m_targetRotationCamera;

    Collider m_sphereCam;

    public override void Start()
    {
        base.Start();
        m_type = States.TRANSIT;

        m_sphereCam = m_variableCam.m_cam.GetComponent<Collider>();
    }

    //Main camera update. Returns true if a change in state ocurred (in order to call OnExit() and OnEnter())
    public override bool OnUpdate(float axisHorizontal, float axisVertical, bool returnCam, float timeStep)
    {
        bool ret = false;

        if (m_changeWithoutTransition)
        {
            if (m_localPivotTransforms)
            {
                m_variableCam.m_pivot.localRotation = m_targetRotationPivot;
                m_variableCam.m_pivot.localPosition = m_targetPivotPosition;
            }
            else
            {
                m_variableCam.m_pivot.rotation = m_targetRotationPivot;
                m_variableCam.m_pivot.position = m_targetPivotPosition;
            }

            m_variableCam.m_cam.localRotation = m_targetRotationCamera;
            m_variableCam.m_cam.localPosition = m_targetCamPosition;

            m_variableCam.m_currentState = m_finalState;
            ret = true;
        }
        else
        {
            m_time += timeStep;
            float perc = m_time / (m_timeBetweenChanges - m_addedTime);
            if (perc >= 1.0f)
                perc = 1.0f;

            Quaternion newPivotRotation = Quaternion.Slerp(m_initialRotationPivot, m_targetRotationPivot, perc);
            Quaternion newCameraRotation = Quaternion.Slerp(m_initialRotationCamera, m_targetRotationCamera, perc);
            Vector3 newPosition = Vector3.Lerp(m_initialCamPosition, m_targetCamPosition, perc);
            Vector3 newPositionPivot = Vector3.Lerp(m_initialPivotPosition, m_targetPivotPosition, perc);

            if (m_localPivotTransforms)
            {
                m_variableCam.m_pivot.localRotation = newPivotRotation;
                m_variableCam.m_pivot.localPosition = newPositionPivot;
            }
            else
            {
                m_variableCam.m_pivot.rotation = newPivotRotation;
                m_variableCam.m_pivot.position = newPositionPivot;
            }

            m_variableCam.m_cam.localRotation = newCameraRotation;
            m_variableCam.m_cam.localPosition = newPosition;

            if (perc >= 1.0f)
            {
                m_variableCam.m_currentState = m_finalState;
                ret = true;
            }
        }

        return ret;
    }

    public override void OnEnter()
    {
        m_time = 0.0f;
        m_addedTime = 0.0f;
        m_transitionStopped = false;
        //m_sphereCam.enabled = true;
    }

    public override void OnExit()
    {
        //m_sphereCam.enabled = false;
    }

    public void ResetTime()
    {
        m_addedTime = m_timeBetweenChanges - m_time;
        m_time = 0.0f;
    }

    public void SetTransitionValues(CameraStates finalState, Vector3 targetCamPosition, Vector3 targetPivotPosition, Quaternion targetRotationCamera, Quaternion targetRotationPivot, bool localPivotPositions, float changeTotalTime)
    {
        m_localPivotTransforms = localPivotPositions;
        if (m_localPivotTransforms)
        {
            m_initialRotationPivot = m_variableCam.m_pivot.localRotation;
            m_initialPivotPosition = m_variableCam.m_pivot.localPosition;
        }
        else
        {
            m_initialRotationPivot = m_variableCam.m_pivot.rotation;
            m_initialPivotPosition = m_variableCam.m_pivot.position;
        }

        m_initialRotationCamera = m_variableCam.m_cam.localRotation;
        m_initialCamPosition = m_variableCam.m_cam.localPosition;

        m_finalState = finalState;
        m_targetRotationCamera = targetRotationCamera;
        m_targetRotationPivot = targetRotationPivot;
        m_targetCamPosition = targetCamPosition;
        m_targetPivotPosition = targetPivotPosition;

        if (changeTotalTime > 0.0f)
        {
            m_changeWithoutTransition = false;
            m_timeBetweenChanges = changeTotalTime;
        } 
        else
        {
            m_changeWithoutTransition = true;
        }        
    }
}
