using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SenceTint : MonoBehaviour
{
    [SerializeField] Color unTintedColor;
    [SerializeField] Color TintedColor;
    float f;
    public float speed  = 0.5f;
    Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
        f = 0f;
    }
    public void Tint()
    {
        StopAllCoroutines();
        f= 0f;
        StartCoroutine(TintScreen());
    }
    public void UnTint()
    {
        StopAllCoroutines();
        f = 0f;
        StartCoroutine(UnTintScreen());
    }
    private IEnumerator TintScreen()
    {
        while(f < 1f)
        {
            f += Time.deltaTime * speed;
            f = Mathf.Clamp(f, 0f, 1f);

            Color color = image.color;
            color = Color.Lerp(unTintedColor, TintedColor  , f);
            image.color = color;

            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator UnTintScreen()
    {
        while (f < 1f)
        {
            f += Time.deltaTime;
            f = Mathf.Clamp(f, 0f, 1f);

            Color color = image.color;
            color = Color.Lerp(TintedColor, unTintedColor, f);
            image.color = color;

            yield return new WaitForEndOfFrame();
        }
    }

}
