using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Slider bar;

    public void Set(int curr, int max)
    {
        bar.maxValue = max;
        bar.value = curr;

        text.text = curr.ToString() + "/" + max.ToString();
    }
}
