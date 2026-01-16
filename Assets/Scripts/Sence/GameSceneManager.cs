using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] SenceTint senceTint;

    [SerializeField] CameraConfiner cameraConfiner;
    string currentScene;

    AsyncOperation unLoad;
    AsyncOperation load;

    bool respawnTransition;
    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
    }

    public void InitSwitchSence(string sceneName, Vector3 target)
    {
        StartCoroutine(SceneTransition(sceneName, target));
    }
    internal void Respawn(Vector3 respawnPointPositon, string respawnPonitScene)
    {
        respawnTransition = true;
        if(currentScene != respawnPonitScene)
        {
            InitSwitchSence(respawnPonitScene, respawnPointPositon);
        }
        else
        {
            MovePlayer(respawnPointPositon);
        }
    }
    IEnumerator SceneTransition(string senceName, Vector3 target)
    {
        senceTint.Tint();

        yield return new WaitForSeconds(1f / senceTint.speed + 0.1f);

        SwitchScene(senceName, target);

        while(load != null && unLoad != null)
        {
            if (load.isDone) { load = null; }
            if (unLoad.isDone) { unLoad = null; }
            yield return new WaitForSeconds(0.1f);

        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentScene));
        cameraConfiner.UpdateBound();
        senceTint.UnTint();   
    }
    public void SwitchScene(string sceneName, Vector3 target)
    {
        load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        unLoad = SceneManager.UnloadSceneAsync(currentScene);
        currentScene = sceneName;
        MovePlayer(target);

    }

    private void MovePlayer(Vector3 target)
    {
        Transform playerSpawnTransform = GameManager.Instance.m_PlayerController.transform;

        Cinemachine.CinemachineBrain currentCamera = Camera.main.GetComponent<CinemachineBrain>();

        currentCamera.ActiveVirtualCamera.OnTargetObjectWarped(
            playerSpawnTransform,
            target - playerSpawnTransform.position);

        playerSpawnTransform.position = new Vector3(
            target.x, target.y, playerSpawnTransform.position.z);
        if (respawnTransition)
        {
            playerSpawnTransform.GetComponent<Player>().FullHeal();
            playerSpawnTransform.GetComponent<Player>().FullRest(0);
            playerSpawnTransform.GetComponent<DisableControls>().EnableControl();
            respawnTransition = false;
        }
    }

}
