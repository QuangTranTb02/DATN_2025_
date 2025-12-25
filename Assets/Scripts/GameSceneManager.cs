using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : Singleton<GameSceneManager>
{
    internal bool IsInGameplay()
    {
        throw new NotImplementedException();
    }

    internal void LoadHub()
    {
        SceneManager.LoadScene(UIManager.Instance.levelSelectScene);
    }
    
    internal void LoadMenuScene()
    {
        SceneManager.LoadScene(UIManager.Instance.mainMenuScene);
    }

    internal void QuitGame()
    {
        Application.Quit();
    }

    internal void RestartCurrentLevel()
    {
        throw new NotImplementedException();
    }
}
