using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : Singleton<LightsManager>
{
	public GameObject _sunMoonPrefabs;
	Light SunLight;

	DayStatesProperties CurrentDayState;
    DayStatesProperties NextDayState;
    float CurrentTimeRatio;

	[Header("Day Options")]
	public Material _skyboxMaterialDay;
	public Color _ambientColorDay;
	public float _sunLightIntensity = 1;
	Coroutine _DayCoroutine;

	[Header("Night Options")]
	public Material _skyboxMaterialNight;
	public Color _ambientColorNight;
	public float _moonLightIntensity = 0.5f;
	Coroutine _NightCoroutine;

	// IN PROGRESS
	// Attention Oliv, je me suis raccrocher à tes events et CurrentTimeRatio, j'en ai besoin
	//
	//
	//
	// ENJOYYYYYYYY


    protected override void Awake()
    {
        base.Awake();

        AmbiantManager.I.onDayStateChanged.AddListener(DayChanged);
		SunLight = _sunMoonPrefabs.GetComponent<Light>();
	}

    protected void Update()
    {
        CurrentTimeRatio = (Time.time - CurrentDayState.TimeStateChanged) / CurrentDayState.DayStateDurationInSecond;
		DayNightCycle();

		//InterpolateLightColor();
	}

	void DayChanged(DayStatesProperties CurrentState, DayStatesProperties NextState)
    {
        CurrentDayState = CurrentState;
        NextDayState = NextState;
    }

	void DayNightCycle()
	{
		if(CurrentDayState.State == DayState.Day)
			_DayCoroutine = StartCoroutine(DayCycle());

		if (CurrentDayState.State == DayState.Night)
			_NightCoroutine = StartCoroutine(NightCycle());

		//Sun.transform.eulerAngles = Vector3.Lerp(CurrentDayState.EnterSunRotation, CurrentDayState.ExitSunRotation, CurrentTimeRatio);
		//Sun.transform.localRotation = Quaternion.Euler(Vector3.Lerp(CurrentDayState.EnterSunRotation, CurrentDayState.ExitSunRotation, CurrentTimeRatio));
		//Moon.transform.localRotation = Quaternion.Euler(Vector3.Lerp(NextDayState.EnterSunRotation, NextDayState.ExitSunRotation, CurrentTimeRatio));
	}

	IEnumerator DayCycle()
	{
		Debug.Log("Début jour");

		ResetSunRotation();

		RenderSettings.skybox = _skyboxMaterialDay;
		SunLight.color = _ambientColorDay;
		SunLight.intensity = _sunLightIntensity;

		while (CurrentTimeRatio < 1)
		{
			_sunMoonPrefabs.transform.eulerAngles = new Vector3(CurrentTimeRatio * 180, 0, 0);
			break;
		}

		Debug.Log("Fin jour");
		_DayCoroutine = null;
		yield return null;
	}

	IEnumerator NightCycle()
	{
		Debug.Log("Début Nuit");

		ResetSunRotation();

		RenderSettings.skybox = _skyboxMaterialNight;
		SunLight.color = _ambientColorNight;
		SunLight.intensity = _moonLightIntensity;
		while (CurrentTimeRatio < 1)
		{
			_sunMoonPrefabs.transform.eulerAngles = new Vector3(CurrentTimeRatio * 180, 0, 0);
			break;
		}

		Debug.Log("Fin Nuit");
		_NightCoroutine = null;
		yield return null;
	}

	void ResetSunRotation()
	{
		_sunMoonPrefabs.transform.eulerAngles = Vector3.zero;
	}



    /*void InterpolateLightColor()
    //{
    //    SunLight.color = Color.Lerp(CurrentDayState.StateColor, NextDayState.StateColor, CurrentTimeRatio);
    //}
	*/

	/*void SkyboxMatGoToDay()
	//{
	//	Debug.Log("Lerp vers jour");
	//	RenderSettings.skybox.Lerp(_skyboxMaterialNight, _skyboxMaterialDay, CurrentTimeRatio);
	//	// Lerp Color Sun
	//}
	//void SkyboxMatGoToNight()
	//{
	//	Debug.Log("Lerp vers nuit");
	//	RenderSettings.skybox.Lerp(_skyboxMaterialDay, _skyboxMaterialNight, CurrentTimeRatio);
	//	// Lerp Color Sun
	//}
	*/
}
