using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : Singleton<LightsManager>
{
    Light GlobalLight;
    DayStatesProperties CurrentDayState;
    DayStatesProperties NextDayState;

	GameObject Sun;
	public float _speedSunRotation = 1;
    public float CurrentTimeRatio;

    protected override void Awake()
    {
		Sun = GameObject.Find("Sun");

        base.Awake();
		GlobalLight = Sun.GetComponent<Light>();
        AmbiantManager.I.onDayStateChanged.AddListener(DayChanged);

    }

    void DayChanged(DayStatesProperties CurrentState, DayStatesProperties NextState)
    {
        CurrentDayState = CurrentState;
        NextDayState = NextState;
    }

    protected void Update()
    {
        CurrentTimeRatio = (Time.time - CurrentDayState.TimeStateChanged) / CurrentDayState.DayStateDurationInSecond;

        InterpolateLightColor();
		SunRotation();
	}

    void InterpolateLightColor()
    {
        GlobalLight.color = Color.Lerp(CurrentDayState.StateColor, NextDayState.StateColor, CurrentTimeRatio);
    }

	void SunRotation()
	{
		Sun.transform.eulerAngles = Vector3.Lerp(CurrentDayState.SunRotation, NextDayState.SunRotation, CurrentTimeRatio);
        //Sun.transform.Rotate(new Vector3(_speedSunRotation * Time.deltaTime, 0, 0));
	}
}
