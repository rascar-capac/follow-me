using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : Singleton<LightsManager>
{
    Light GlobalLight;
    public Color DayColor;
    public Color NightColor;

    // Start is called before the first frame update
    protected override void Start()
    {
        GlobalLight = FindObjectOfType<Light>();
        GameManager.I.onDayStateChanged.AddListener(DayChanged);
    }

    void DayChanged(DayStatesProperties CurrentState)
    {
        if (CurrentState.State == DayState.Day)
            GlobalLight.color = DayColor;
        else if (CurrentState.State == DayState.Night)
            GlobalLight.color = NightColor;
    }
}
