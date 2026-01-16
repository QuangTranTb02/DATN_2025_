using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    [SerializeField] GameObject Hightlighter;
    GameObject CurrentTarget;
    public void Highlight(GameObject target)
    {
        if (CurrentTarget == target)
        {
            return;
        }
        CurrentTarget = target;
        Vector3 position = target.transform.position + Vector3.up;
        Highlight(position);
    }
    public void Highlight(Vector3 position)
    {
        Hightlighter.SetActive(true);
        Hightlighter.transform.position = position;
    }
    public void Hide()
    {
        CurrentTarget = null;
        Hightlighter?.SetActive(false);
    }
}
    