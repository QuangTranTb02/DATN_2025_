using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Currence : MonoBehaviour
{
    [SerializeField] int amount;
    [SerializeField] TextMeshProUGUI textMeshPro;
    public void Add(int moneyGain)
    {
        amount += moneyGain;
        UpdateText();
    }

    internal bool Check(int totolPrice)
    {
        return amount >= totolPrice;
    }

    internal void Decrease(int totolPrice)
    {
        amount -= totolPrice;
        if (amount < 0) { amount = 0; }
        UpdateText();
    }

    private void Start()
    {
        amount = 10000000;
        UpdateText();
    }

    private void UpdateText()
    {
        textMeshPro.text = amount.ToString();
    }
}
