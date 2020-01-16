using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : Singleton<LightsManager>
{
    Light GlobalLight;
    DayStatesProperties CurrentDayState;
    DayStatesProperties NextDayState;

	GameObject _sunGO;
	public float _speedSunRotation = 1;


    protected override void Awake()
    {
		_sunGO = GameObject.Find("Sun");

        base.Awake();
		//GlobalLight = FindObjectOfType<Light>();
		GlobalLight = _sunGO.GetComponent<Light>();
        GameManager.I.onDayStateChanged.AddListener(DayChanged);

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
		//InterpolateLightColor();
		SunRotation();
	}

    void InterpolateLightColor()
    {
        float t = (Time.time - CurrentDayState.TimeStateChanged) / CurrentDayState.DayStateDurationInSecond;
        GlobalLight.color = Color.Lerp(CurrentDayState.StateColor, NextDayState.StateColor, t);
    }

	void SunRotation()
	{
		_sunGO.transform.Rotate(new Vector3(_speedSunRotation * Time.deltaTime, 0, 0));

		if (_sunGO.transform.eulerAngles.x < 180)
			Debug.Log("Jour");
		if (_sunGO.transform.eulerAngles.x > 180)
			Debug.Log("Nuit");
	}
}
