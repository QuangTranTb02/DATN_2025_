using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRepawn : MonoBehaviour
{
    [SerializeField] Vector3 respawnPointPositon;
    [SerializeField] string respawnPonitScene;
    internal void StartRespawn()
    {
        GameSceneManager.Instance.Respawn(respawnPointPositon,respawnPonitScene);
    }

   
}
