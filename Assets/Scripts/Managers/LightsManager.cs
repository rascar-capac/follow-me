using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct DayNightCycleProperties
{
	[InfoBox("Do not forget to also modify the material properties")]
	public Material SkyboxMaterial;
	public Color AmbientColor;
	public float SunLightIntensity;
}

public class LightsManager : Singleton<LightsManager>
{
	public Light SunLight;

	[Title("Day Options")]
	public DayNightCycleProperties DayProperties;

	[Title("Night Options")]
	public DayNightCycleProperties NightProperties;

	DayStatesProperties CurrentDayState;
    float CurrentTimeRatio;

	//Il reste des Lerps à faire lors des transitions jour/nuit entre:
	//  - les 2 materials (jour et Nuit)
	//  - les 2 colors _ambientColor....
	//  - l'intensité de la Light _.....LightIntensity


	protected override void Start()
    {
        base.Start();

        AmbiantManager.I.onDayStateChanged.AddListener(DayChanged);

		//Debug.Log(DayProperties.SkyboxMaterial.GetFloat("_SunSize")); //Tips: Access to Material Properties.
	}

    protected void Update()
    {
        CurrentTimeRatio = (Time.time - CurrentDayState.TimeStateChanged) / CurrentDayState.DayStateDurationInSecond;

		//InterpolateLightColor();
	}

	void DayChanged(DayStatesProperties currentDayState, DayStatesProperties nextDayState)
    {
		CurrentDayState = currentDayState;

		StopAllCoroutines();

		if (CurrentDayState.State == DayState.Day)
			StartCoroutine(DayCycle());

		else if (CurrentDayState.State == DayState.Night)
			StartCoroutine(NightCycle());
	}

	IEnumerator DayCycle()
	{
		ResetSunRotation();

		RenderSettings.skybox = DayProperties.SkyboxMaterial;
		SunLight.color = DayProperties.AmbientColor;
		SunLight.intensity = DayProperties.SunLightIntensity;

		while (CurrentTimeRatio < 1)
		{
			RotateLight();
			yield return null;
		}
	}

	IEnumerator NightCycle()
	{
		ResetSunRotation();

		RenderSettings.skybox = NightProperties.SkyboxMaterial;
		SunLight.color = NightProperties.AmbientColor;
		SunLight.intensity = NightProperties.SunLightIntensity;

		while (CurrentTimeRatio < 1)
		{
			RotateLight();
			yield return null;
		}
	}

	void RotateLight()
	{
		SunLight.transform.eulerAngles = new Vector3(CurrentTimeRatio * 180, 0, 0);
	}

	void ResetSunRotation()
	{
		SunLight.transform.eulerAngles = Vector3.zero;
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
