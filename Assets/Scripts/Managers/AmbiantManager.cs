﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Borodar.FarlandSkies.LowPoly;

public class AmbiantManager : Singleton<AmbiantManager>
{
    public List<Material> MaterialReferences;


	public HourChangedEvent onHourChanged = new HourChangedEvent();
    public DayStateHasChanged onDayStateHasChanged = new DayStateHasChanged();
    public TimePhaseChangedEvent onTimePhaseChanged = new TimePhaseChangedEvent();
    public float _LastHour;
    GameObject Terrain;
    private int PhaseIndex = -1;

    protected override void Start()
    {
        base.Start();
        Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        Terrain = (GameObject)ObjectsManager.I["Terrain"];
    }

    public DayState currentStateOfDay = DayState.Day;
    private void Update()
    {
        //UIManager.I.SetTimeOfDay();

        List<TimePhase> phases = GameManager.I._data.Phases;
        float time = SkyboxDayNightCycle.Instance.TimeOfDay;
        for (int i = 0 ; i < phases.Count ; i++)
        {
            int previousIndex = (int) Mathf.Repeat(i - 1, phases.Count);
            if (phases[i].endingPercentage > phases[previousIndex].endingPercentage)
            {
                if (time > phases[i].endingPercentage)
                {
                    continue;
                }
            }
            if (time > phases[i].endingPercentage && time < phases[previousIndex].endingPercentage)
            {
                continue;
            }

            if (i != PhaseIndex)
            {
                PhaseIndex = i;
                onTimePhaseChanged.Invoke(PhaseIndex);
            }
            break;
        }

        if (SkyboxDayNightCycle.Instance.TimeOfDay >= SkyboxDayNightCycle.Instance._sunrise && SkyboxDayNightCycle.Instance.TimeOfDay <= SkyboxDayNightCycle.Instance._sunset)
        {
            if (currentStateOfDay != DayState.Day)
            {
                Debug.Log("Day");
                currentStateOfDay = DayState.Day;
                onDayStateHasChanged.Invoke(currentStateOfDay);
            }
        }
        else if (currentStateOfDay != DayState.Night)
        {
            Debug.Log("Night");
            currentStateOfDay = DayState.Night;
            onDayStateHasChanged.Invoke(currentStateOfDay);
        }
        SkyboxController.Instance.CloudsRotation = 360 * SkyboxDayNightCycle.Instance.TimeOfDay / 100;
        if (MaterialReferences != null  && MaterialReferences.Count > 0)
        {
            for (int i = 0; i < MaterialReferences.Count ; i++)
            {
                MaterialReferences[i].SetFloat("_DayNightEmissive", 1- SkyboxDayNightCycle.Instance.TimeOfDay / 100);
                MaterialReferences[i].SetFloat("_DayNightFresnel", 1- SkyboxDayNightCycle.Instance.TimeOfDay / 100);
                MaterialReferences[i].SetFloat("_DayNightAlbedo", 1- SkyboxDayNightCycle.Instance.TimeOfDay / 100);

            }
        }
    }

    void ChangeMaterial(DayState state)
    {
        //UIManager.I.AlertMessage("It is " + currentDayStateProperties.State.ToString());


        //string Suffix = "D";
        //List<Material> SearchList = Materials.DayMaterials;
        //if (currentDayStateProperties.State == DayState.Night)
        //{
        //    Suffix = "N";
        //    SearchList = Materials.NightMaterials;
        //}

        //Renderer childRenderer = Terrain.transform.GetComponent<Renderer>();
        //Material TerrainMaterial = GetTargetMaterial(childRenderer, Suffix, SearchList);
        //if (TerrainMaterial)
        //{
        //    childRenderer.material.SetTexture("_MainTex", TerrainMaterial.GetTexture("_MainTex"));
        //    childRenderer.material.SetTexture("_BaseBump", TerrainMaterial.GetTexture("_BaseBump"));
        //    childRenderer.material.SetTexture("_Texture1", TerrainMaterial.GetTexture("_Texture1"));
        //    childRenderer.material.SetTexture("_Texture2", TerrainMaterial.GetTexture("_Texture2"));
        //    childRenderer.material.SetTexture("_Texture3", TerrainMaterial.GetTexture("_Texture3"));
        //}

        //for (int i = 0; i < Terrain.transform.childCount; i++)
        //{
        //    childRenderer = Terrain.transform.GetChild(i).GetComponent<Renderer>();
        //    ChangeMaterialSettings(childRenderer, GetTargetMaterial(childRenderer, Suffix, SearchList));
        //}
    }

    Material GetTargetMaterial(Renderer renderer, string Suffix, List<Material> SearchList)
    {
        string matName = renderer.material.name.Replace("(Instance)", "").Trim();
        if (!matName.EndsWith("_D"))
            return null;

        string otherName = matName.Substring(0, matName.Length - 1) + Suffix;

        return SearchList.Find(m => m.name == otherName);
    }

    public void ChangeMaterialSettings(Renderer renderer, Material other)
    {
        if (other)
        {
            if (other.IsKeywordEnabled("_EMISSION"))
                renderer.material.EnableKeyword("_EMISSION");
            else
                renderer.material.DisableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", other.GetColor("_EmissionColor"));
            renderer.material.SetColor("_Color", other.GetColor("_Color"));
            renderer.material.SetTexture("_MainTex", other.GetTexture("_MainTex"));
            renderer.material.SetTexture("_EmissionMap", other.GetTexture("_EmissionMap"));
        }
    }

	void ResetLastHour(DayStatesProperties currentDayStateProperties, DayStatesProperties nextDayStateProperties)
	{
		_LastHour = 0;
		// Invoke at 00h00
		onHourChanged.Invoke(Mathf.RoundToInt(_LastHour), CurrentDayState.State);
	}

	public bool IsUsableNow(List<DayState> states)
    {
        if (states != null)
        {
            int i = 0;
            for (i = 0; i < states.Count; i++)
                if (states[i] == AmbiantManager.I.CurrentDayState.State)
                    break;
            if (i >= states.Count)
                return false;
        }
        return true;
    }

	#region Fog
	//GameObject Fog;
    Zone FogZone;
    Player Player;
    //ParticleSystem FogParticles;


    //void StartFog()
    //{
    //    if (GameManager.I._data.MinimumTimeBetweenFog == 0)
    //        return;
    //    Fog.SetActive(true);
    //    Fog.transform.position = new Vector3(CameraManager.I._MainCamera.transform.position.x, CameraManager.I._MainCamera.transform.position.y, CameraManager.I._MainCamera.transform.position.z);// + Vector3.ProjectOnPlane(Player.transform.forward, Vector3.up);
    //    FogParticles.Play();
    //    StartChrono(GameManager.I._data.MinimumTimeBetweenFog, EndFog);
    //    Debug.Log("Fog on");
    //}
    //void EndFog()
    //{
    //    if (GameManager.I._data.MinimumTimeBetweenFog == 0)
    //        return;
    //    FogParticles.Stop();
    //    Fog.SetActive(false) ;
    //    Player.ExitZone(FogZone);
    //    StartChrono(GameManager.I._data.MinimumTimeBetweenFog, StartFog);
    //    Debug.Log("Fog off");
    //}
    #endregion

	#region Day States Management
	public DayStateChangedEvent onDayStateChanged = new DayStateChangedEvent();
    public int CurrentDayStateIndex = 0;
    public DayStatesProperties CurrentDayState => GameManager.I._data.States[CurrentDayStateIndex];
    public DayStatesProperties NextDayState => GameManager.I._data.States[(int)Mathf.Repeat(CurrentDayStateIndex + 1, GameManager.I._data.States.Length)];
    public bool IsDay => CurrentDayState.State == DayState.Day;
    public bool IsNight => CurrentDayState.State == DayState.Night;
    public float CurrentTimeOfDay;
    public float TotalDayTime;
    void InitDaysStates()
    {
        TotalDayTime = GameManager.I._data.States[0].StateHoursCount + GameManager.I._data.States[1].StateHoursCount;

        CurrentDayStateIndex = 1;
    }
    void ChangeDayState()
    {
        CurrentDayStateIndex++;

        CurrentDayStateIndex = (int)Mathf.Repeat(CurrentDayStateIndex, GameManager.I._data.States.Length);
        GameManager.I._data.States[CurrentDayStateIndex].TimeStateChanged = Time.time;
        DayStatesProperties NextState = GameManager.I._data.States[(int)Mathf.Repeat(CurrentDayStateIndex + 1, GameManager.I._data.States.Length)];
        onDayStateChanged?.Invoke(GameManager.I._data.States[CurrentDayStateIndex], NextState);

        StartChrono(GameManager.I._data.States[CurrentDayStateIndex].DayStateDurationInSecond, ChangeDayState, DayTimeCallback);
    }
    void DayTimeCallback(float timeElapsed, float timeRatio)
    {
        CurrentTimeOfDay = Mathf.Lerp(0, CurrentDayState.StateHoursCount, timeRatio);
    }
    #endregion
}
public class DayStateChangedEvent : UnityEvent<DayStatesProperties, DayStatesProperties> { }
public class HourChangedEvent : UnityEvent<int, DayState> { }
public class DayStateHasChanged : UnityEvent<DayState> { }

public class TimePhaseChangedEvent : UnityEvent<int> { }
