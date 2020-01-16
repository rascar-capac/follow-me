using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : Singleton<LightsManager>
{
    Light GlobalLight;
    DayStatesProperties CurrentDayState;
    DayStatesProperties NextDayState;

    protected override void Awake()
    {
        base.Awake();
        GlobalLight = FindObjectOfType<Light>();
        AmbiantManager.I.onDayStateChanged.AddListener(DayChanged);
    }
    // Start is called before the first frame update
    //protected override void Start()
    //{
      
    //}

    void DayChanged(DayStatesProperties CurrentState, DayStatesProperties NextState)
    {
        CurrentDayState = CurrentState;
        NextDayState = NextState;
    }

    protected void Update()
    {
        InterpolateLightColor();
    }

    void InterpolateLightColor()
    {
        float t = (Time.time - CurrentDayState.TimeStateChanged) / CurrentDayState.DayStateDurationInSecond;
        GlobalLight.color = Color.Lerp(CurrentDayState.StateColor, NextDayState.StateColor, t);
    }
}
