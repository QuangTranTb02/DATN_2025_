using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelNameText;

    public LevelData LevelData { get; set; }

    private Button _button;
    private Image _image;

    public Color ReturnColor { get; set; }

    private void Awake()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
        ReturnColor = Color.grey;
    }
    public void Setup(LevelData level, bool isUnlocked)
    {
        LevelData = level;
        _levelNameText.SetText(level.levelID);  

        _button.interactable = isUnlocked;

        if (isUnlocked)
        {
            _button.onClick.AddListener(LoadLevel);
            ReturnColor = Color.white;
            _image.color = ReturnColor;
        }
        else
        {
            ReturnColor = Color.grey;
            _image.color = ReturnColor;
        }
    }

    public void Unlock()
    {
        _button.interactable = true;
        _button.onClick.AddListener(LoadLevel);
        ReturnColor = Color.white;
        _image.color = ReturnColor;
    }

    private void LoadLevel()
    {
        LevelManager.Instance.SetCurrentLevelIndex(LevelData.levelNumber);
        LevelManager.Instance.LoadLevel(LevelData.levelNumber);
        SceneManager.LoadScene(LevelData.Scene);
    }
}
