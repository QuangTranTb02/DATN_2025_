using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI collectiblesText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI DeathText;
    [SerializeField] private TextMeshProUGUI LevelNameText;

    private void Update()
    {
        LevelNameText.text = LevelManager.Instance.GetLevelName();
        UpdateTimer(LevelManager.Instance.GetLevelTime());
    }
    

    public void UpdateTimer(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}