using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct DayNightCycleProperties
{
	[InfoBox("Do not forget to also modify the material properties")]
	public Material SkyboxMaterial;
	public float AmbientLightIntensity;
	public Color SunColor;
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


	protected override void Start()
    {
        base.Start();

        AmbiantManager.I.onDayStateChanged.AddListener(DayChanged);
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
		RenderSettings.ambientIntensity = DayProperties.AmbientLightIntensity;

		SunLight.color = DayProperties.SunColor;
		SunLight.intensity = DayProperties.SunLightIntensity;

		while (CurrentTimeRatio < 1)
		{
			RotateLight();

			//Lerp test...
			//RenderSettings.skybox.SetColor("_SkyTint", Color.Lerp(DayProperties.AmbientColor, NightProperties.AmbientColor, CurrentTimeRatio));
			//RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(DayProperties.SkyboxMaterial.GetFloat("_Exposure"), NightProperties.SkyboxMaterial.GetFloat("_Exposure"), CurrentTimeRatio));
			//Skybox.material.Lerp(DayProperties.SkyboxMaterial, NightProperties.SkyboxMaterial, CurrentTimeRatio);
			//SunLight.color = Color.Lerp(Color.blue, Color.red, CurrentTimeRatio);
			//RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, 0.2f, Time.deltaTime);
			yield return null;
		}
	}

	IEnumerator NightCycle()
	{
		ResetSunRotation();

		RenderSettings.skybox = NightProperties.SkyboxMaterial;
		RenderSettings.ambientIntensity = NightProperties.AmbientLightIntensity;

		SunLight.color = NightProperties.SunColor;
		SunLight.intensity = NightProperties.SunLightIntensity;

		while (CurrentTimeRatio < 1)
		{
			RotateLight();

			// Lerp test...
			//RenderSettings.skybox.SetColor("_SkyTint", Color.Lerp(NightProperties.AmbientColor, DayProperties.AmbientColor, CurrentTimeRatio));
			//RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(NightProperties.SkyboxMaterial.GetFloat("_Exposure"), DayProperties.SkyboxMaterial.GetFloat("_Exposure"), CurrentTimeRatio));
			//Skybox.material.Lerp(NightProperties.SkyboxMaterial, DayProperties.SkyboxMaterial, CurrentTimeRatio);
			//SunLight.color = Color.Lerp(Color.red, Color.blue, CurrentTimeRatio);
			//RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, 1, Time.deltaTime);
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

}
