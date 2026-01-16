using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionType
{
    Warp,
    Sence
}
public class Transition : MonoBehaviour
{
    [SerializeField] TransitionType transitionType;
    [SerializeField] string sceneNameToTransition;
    [SerializeField] Vector3 targetPositon;
    [SerializeField] Collider2D confiner;
    [SerializeField] Transform destination;

    CameraConfiner CameraConfiner;

    
    void Start()
    {
        if(confiner != null)
        {
            CameraConfiner = FindObjectOfType<CameraConfiner>();
        }
    }
    internal void InitTransition(Transform toTransition)
    {
        
        switch (transitionType)
        {
            case TransitionType.Warp:
                Cinemachine.CinemachineBrain currentCamera = Camera.main.GetComponent<CinemachineBrain>();
                if (CameraConfiner != null)
                {
                    CameraConfiner.UpdateBound(confiner);
                }
                currentCamera.ActiveVirtualCamera.OnTargetObjectWarped(toTransition,
            destination.position - toTransition.position);
                toTransition.position = new Vector3(
                                  destination.position.x,
                                  destination.position.y,
                                  destination.position.z);
                break;
            case TransitionType.Sence:
                GameSceneManager.Instance.InitSwitchSence(sceneNameToTransition, targetPositon);
                break;
        }
    }
    private void OnDrawGizmos()
    {
        if(transitionType == TransitionType.Sence) {
            Handles.Label(transform.position, " to " + sceneNameToTransition);
        }
        if(transitionType == TransitionType.Warp)
        {
            Gizmos.DrawLine(transform.position, destination.position);
        }
        
    }
}
