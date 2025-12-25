using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controller cho Level Complete Screen
/// Shows: Stars, Time, Deaths, Best Time
/// Buttons: Evaluate (next level?), Retry
/// </summary>
public class LevelCompleteController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI starText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private Image[] starImages; // 3 stars

    private LevelData currentLevel;

    public void Initialize(LevelData levelData)
    {
        currentLevel = levelData;

        DisplayStats();
        PlayAnimation();
    }

    private void DisplayStats()
    {
        if (currentLevel == null) return;
        // Time
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(currentLevel.bestTime / 60f);
            int seconds = Mathf.FloorToInt(currentLevel.bestTime % 60f);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }

        // Deaths
        if (deathText != null)
        {
            deathText.text = currentLevel.totalDeaths.ToString();
        }

        // Best Time
        if (bestTimeText != null)
        {
            int minutes = Mathf.FloorToInt(currentLevel.bestTime / 60f);
            int seconds = Mathf.FloorToInt(currentLevel.bestTime % 60f);
            bestTimeText.text = $"{minutes:00}:{seconds:00}";
        }

        // Stars
        if (starText != null)
        {
            starText.text = $"{currentLevel.stars}/3";
        }
    }


    #region Button Callbacks
   
    #endregion

    #region Animations

    private void PlayAnimation()
    {
        // Panel scale animation
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.5f)
            .setEaseOutBack()
            .setIgnoreTimeScale(true);

        // Stars appear one by one
        if (starImages != null)
        {
            for (int i = 0; i < currentLevel.stars && i < starImages.Length; i++)
            {
                if (starImages[i] == null) continue;

                int index = i;
                starImages[index].transform.localScale = Vector3.zero;

                LeanTween.scale(starImages[index].gameObject, Vector3.one, 0.5f)
                    .setDelay(0.3f + (0.2f * index))
                    .setEaseOutBack()
                    .setIgnoreTimeScale(true)
                    .setOnComplete(() => {
                        // Play star sound
                        AudioManager.Instance?.PlaySFX("StarCollect");
                    });
            }
        }
    }

    #endregion

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}