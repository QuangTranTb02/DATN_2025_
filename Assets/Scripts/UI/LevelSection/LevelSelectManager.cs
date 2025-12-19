using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    public Transform LevelParent;
    public GameObject LevelButtonPrefab;
    public TextMeshProUGUI AreaHeaderText;
    public TextMeshProUGUI LevelHeaderText;
    public AreaData CurrentArea;
    public LineRenderer LinePrefab;

    public HashSet<string> UnlockedLevelIDs = new HashSet<string>();

    private LevelSelectEventSystemHandler _eventSystemHandler;

    private Camera _camera;

    private List<GameObject> _buttonObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> _buttonLocations = new Dictionary<GameObject, Vector3>();

    private void Awake()
    {
        _camera = Camera.main;
        _eventSystemHandler = GetComponentInChildren<LevelSelectEventSystemHandler>(true);
    }

    private void Start()
    {
        AssignAreaText();
        LoadUnlockedLevels();
        CreateLevelButtons();
    }
    public void AssignAreaText()
    {
        AreaHeaderText.SetText(CurrentArea.AreaName);
    }
    private void LoadUnlockedLevels()
    {
        foreach (var level in CurrentArea.Levels)
        {
            if (level.isUnlocked)
            {
                UnlockedLevelIDs.Add(level.levelID);
            }
        }
    }

    private void CreateLevelButtons()
    {
        for (int i = 0; i < CurrentArea.Levels.Count; i++)
        {
            GameObject buttonGO = Instantiate(LevelButtonPrefab, LevelParent);
            _buttonObjects.Add(buttonGO);

            RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();

            buttonGO.name = CurrentArea.Levels[i].levelID;
            CurrentArea.Levels[i].LevelButtonObj = buttonGO;

            LevelButton levelButton = buttonGO.GetComponent<LevelButton>();
            levelButton.Setup(CurrentArea.Levels[i], UnlockedLevelIDs.Contains(CurrentArea.Levels[i].levelID));

            // populate the selectables for the event system
            Selectable selectable = buttonGO.GetComponent<Selectable>();
            _eventSystemHandler.AddSelectable(selectable);

            if (i > 0)
            {
                LineRenderer line = Instantiate(LinePrefab, LevelParent);

                line.transform.SetSiblingIndex(0);
                LineRendererConector lineConnector = line.GetComponent<LineRendererConector>();
                lineConnector.StartRectTrans = CurrentArea.Levels[i - 1].LevelButtonObj.GetComponent<RectTransform>();
                lineConnector.EndRectTrans = buttonGO.GetComponent<RectTransform>();

                StartCoroutine(DelayedLineSetup(lineConnector));
            }
        }
        LevelParent.gameObject.SetActive(true);
        _eventSystemHandler.InitSelectables();
        _eventSystemHandler.SetFirstSelected();
    }
    private IEnumerator DelayedLineSetup(LineRendererConector lineConnector)
    {
        yield return null;
        lineConnector.UpdateLinePosition();
    }
}
