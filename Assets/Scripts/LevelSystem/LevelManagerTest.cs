using NUnit.Framework.Internal;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class LevelManagerTest : MonoBehaviour
{
    private void Update()
    {
        if (LevelManager.Instance == null) return;

        // 1 = Load Level 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LevelManager.Instance.LoadLevel(0);
        }

        // 2 = Load Level 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LevelManager.Instance.LoadLevel(1);
        }

        // 3 = Load Level 3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LevelManager.Instance.LoadLevel(2);
        }

        // R = Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            LevelManager.Instance.ReloadCurrentLevel();
        }

        // N = Next Level
        if (Input.GetKeyDown(KeyCode.N))
        {
            LevelManager.Instance.LoadNextLevel();
        }

        // C = Complete Level (cheat)
        if (Input.GetKeyDown(KeyCode.C))
        {
            LevelManager.Instance.CompleteLevel();
        }

        // X = Reset All Progress
        if (Input.GetKeyDown(KeyCode.X))
        {
            LevelManager.Instance.ResetAllProgress();
            Debug.Log("Progress reset!");
        }
    }

    private void OnGUI()
    {
        if (LevelManager.Instance == null) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 400));
        GUILayout.Label("=== LEVEL MANAGER TEST ===");
        GUILayout.Label($"Current Level: {LevelManager.Instance.GetCurrentLevel()?.levelName ?? "None"}");
        GUILayout.Label($"Time: {LevelManager.Instance.GetLevelTime():F1}s");
        GUILayout.Label($"Deaths: {LevelManager.Instance.GetDeathsThisLevel()}");
        GUILayout.Label($"Collectibles: {LevelManager.Instance.GetCollectiblesThisLevel()}");
        GUILayout.Label("");
        GUILayout.Label("Controls:");
        GUILayout.Label("1/2/3 - Load Level");
        GUILayout.Label("R - Reload");
        GUILayout.Label("N - Next Level");
        GUILayout.Label("C - Complete Level");
        GUILayout.Label("X - Reset Progress");
        GUILayout.EndArea();
    }
}
