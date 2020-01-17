using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : Singleton<LightsManager>
{
    Light SunLight;
    Light MoonLight;

    DayStatesProperties CurrentDayState;
    DayStatesProperties NextDayState;

	public GameObject Sun;
    GameObject Moon;
    public float _speedSunRotation = 1;
    public float CurrentTimeRatio;

    protected override void Awake()
    {
        base.Awake();
        //Sun = GameObject.Find("Sun");
        //Moon = GameObject.Find("Moon");

		SunLight = Sun.GetComponent<Light>();
        //MoonLight = Moon.GetComponent<Light>();
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

		//InterpolateLightColor();
		SunRotation();
	}

    void InterpolateLightColor()
    {
        SunLight.color = Color.Lerp(CurrentDayState.StateColor, NextDayState.StateColor, CurrentTimeRatio);
    }

	void SunRotation()
	{
		//Sun.transform.eulerAngles = Vector3.Lerp(CurrentDayState.EnterSunRotation, CurrentDayState.ExitSunRotation, CurrentTimeRatio);
		//Sun.transform.localRotation = Quaternion.Euler(Vector3.Lerp(CurrentDayState.EnterSunRotation, CurrentDayState.ExitSunRotation, CurrentTimeRatio));
		//Moon.transform.localRotation = Quaternion.Euler(Vector3.Lerp(NextDayState.EnterSunRotation, NextDayState.ExitSunRotation, CurrentTimeRatio));

		Sun.transform.Rotate(new Vector3(_speedSunRotation * Time.deltaTime, 0, 0));
	}
}
