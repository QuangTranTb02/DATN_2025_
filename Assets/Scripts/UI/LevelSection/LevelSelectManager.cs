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

            StartCoroutine(AddLocationAfterDelay(buttonGO, buttonRect));

            if (i > 0 && levelButton.LevelData.isUnlocked)
            {
                LineRenderer line = Instantiate(LinePrefab, LevelParent);

                line.transform.SetSiblingIndex(0);
                LineRendererConector lineConnector = line.GetComponent<LineRendererConector>();
                lineConnector.StartRectTrans = CurrentArea.Levels[i - 1].LevelButtonObj.GetComponent<RectTransform>();
                lineConnector.EndRectTrans = buttonGO.GetComponent<RectTransform>();

                StartCoroutine(DelayedLineSetup(lineConnector));
            }
        }
        StartCoroutine(SetupButtonNavigation());
        LevelParent.gameObject.SetActive(true);
        _eventSystemHandler.InitSelectables();
        _eventSystemHandler.SetFirstSelected();
    }
    private IEnumerator DelayedLineSetup(LineRendererConector lineConnector)
    {
        yield return null;
        lineConnector.UpdateLinePosition();
    }
    private IEnumerator AddLocationAfterDelay(GameObject buttonGO, RectTransform buttonRect)
    {
        yield return null;

        Vector2 buttonScreenPoint = RectTransformUtility.WorldToScreenPoint(_camera, buttonRect.position);
        Vector3 buttonWorldPos = _camera.ScreenToWorldPoint(new Vector3(buttonScreenPoint.x, buttonScreenPoint.y, _camera.nearClipPlane));

        _buttonLocations.Add(buttonGO, buttonWorldPos);
    }

    #region Navigation

    private IEnumerator SetupButtonNavigation()
    {
        yield return null;

        for (int i = 0; i < _buttonObjects.Count; i++)
        {
            GameObject currentButton = _buttonObjects[i];
            Vector3 currentPos = _buttonLocations[currentButton];
            Selectable currentSelectable = currentButton.GetComponent<Selectable>();
            Navigation nav = new Navigation { mode = Navigation.Mode.Explicit };

            //check if previous button exists
            if (i > 0 && UnlockedLevelIDs.Contains(CurrentArea.Levels[i].levelID))
            {
                GameObject prevButton = _buttonObjects[i - 1];
                Vector3 prevPos = _buttonLocations[prevButton];
                Vector3 dirToPrev = (prevPos - currentPos).normalized;

                if (Vector3.Dot(dirToPrev, Vector3.right) > 0.7f)
                    nav.selectOnRight = prevButton.GetComponent<Selectable>();
                else if (Vector3.Dot(dirToPrev, Vector3.left) > 0.7f)
                    nav.selectOnLeft = prevButton.GetComponent<Selectable>();
                else if (Vector3.Dot(dirToPrev, Vector3.up) > 0.7f)
                    nav.selectOnUp = prevButton.GetComponent<Selectable>();
                else if (Vector3.Dot(dirToPrev, Vector3.down) > 0.7f)
                    nav.selectOnDown = prevButton.GetComponent<Selectable>();
            }

            //check if future button exists
            if (i < _buttonObjects.Count - 1 && UnlockedLevelIDs.Contains(CurrentArea.Levels[i + 1].levelID))
            {
                GameObject nextButton = _buttonObjects[i + 1];
                Vector3 nextPos = _buttonLocations[nextButton];
                Vector3 dirToNext = (nextPos - currentPos).normalized;

                if (Vector3.Dot(dirToNext, Vector3.right) > 0.7f)
                    nav.selectOnRight = nextButton.GetComponent<Selectable>();
                else if (Vector3.Dot(dirToNext, Vector3.left) > 0.7f)
                    nav.selectOnLeft = nextButton.GetComponent<Selectable>();
                else if (Vector3.Dot(dirToNext, Vector3.up) > 0.7f)
                    nav.selectOnUp = nextButton.GetComponent<Selectable>();
                else if (Vector3.Dot(dirToNext, Vector3.down) > 0.7f)
                    nav.selectOnDown = nextButton.GetComponent<Selectable>();
            }
            currentSelectable.navigation = nav;
        }
    }
    #endregion
}
