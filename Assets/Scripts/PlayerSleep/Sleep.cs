using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : MonoBehaviour
{
    DisableControls disableControls;
    Player player;
    DayTimeController dayTimeController;
    private void Awake()
    {
        disableControls = GetComponent<DisableControls>();
        player = GetComponent<Player>();
        dayTimeController = GameManager.Instance.DayTimeController;
    }
    internal void DoSleep()
    {
        StartCoroutine(SleepRoutine());
    }
    IEnumerator SleepRoutine()
    {
        SenceTint senceTint = GameManager.Instance.screenTint;
        disableControls.DisableControl();
        senceTint.Tint();
        yield return new WaitForSeconds(2f);

        player.FullRest(0);
        player.FullHeal();

        dayTimeController.SkipToMorning();

        senceTint.UnTint();
        yield return new WaitForSeconds(2f);
        disableControls.EnableControl();
        
        yield return null;
    }
}
