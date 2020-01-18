﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum DayState
{
    Day,
    Night,
    _count
}

public struct DayStatesProperties
{
    public DayState State;
    public float DayStateDurationInSecond;
    public float StateHoursCount;
    public Color StateColor;
    public float TimeStateChanged;
    public Vector3 EnterSunRotation;
    public Vector3 ExitSunRotation;
}

public class AmbiantManager : Singleton<AmbiantManager>
{
    #region Fog
    GameObject Fog;
    GameObject Player;
    ParticleSystem FogParticles;
    [Header("Seconds between 2 fogs apparition")]
    public float MinimumTimeBetweenFog = 0;


    void StartFog()
    {
        if (MinimumTimeBetweenFog == 0)
            return;
        Fog.SetActive(true);
        Fog.transform.position = new Vector3(CameraManager.I._MainCamera.transform.position.x, CameraManager.I._MainCamera.transform.position.y, CameraManager.I._MainCamera.transform.position.z);// + Vector3.ProjectOnPlane(Player.transform.forward, Vector3.up);
        FogParticles.Play();
        StartChrono(MinimumTimeBetweenFog, EndFog);
        Debug.Log("Fog on");
    }
    void EndFog()
    {
        if (MinimumTimeBetweenFog == 0)
            return;
        FogParticles.Stop();
        Fog.SetActive(false) ;
        StartChrono(MinimumTimeBetweenFog, StartFog);
        Debug.Log("Fog off");
    }
    #endregion

    protected override void Start()
    {
        base.Start();
        Fog = (GameObject)ObjectsManager.I["Fog"];
        FogParticles = Fog.GetComponentInChildren<ParticleSystem>();
        Fog.SetActive(false);
        Player = (GameObject)ObjectsManager.I["Player"];
        InitDaysStates();
        ChangeDayState();
        StartChrono(MinimumTimeBetweenFog, StartFog);
    }

    #region Day States Management
    public DayStateChangedEvent onDayStateChanged = new DayStateChangedEvent();
    public int CurrentDayStateIndex = 0;
    public DayStatesProperties[] States;
    public DayStatesProperties CurrentDayState => States[CurrentDayStateIndex];
    public DayStatesProperties NextDayState => States[(int)Mathf.Repeat(CurrentDayStateIndex + 1, States.Length)];
    public bool IsDay => CurrentDayState.State == DayState.Day;
    public float CurrentTimeOfDay;
    public float TotalDayTime;
    void InitDaysStates()
    {
        States = new DayStatesProperties[((int)DayState._count)];

        States[0].State = DayState.Day;
        States[0].DayStateDurationInSecond = 20f;
        States[0].StateColor = new Color((float)255 / (float)255, (float)240 / (float)255, (float)210 / (float)255);
        States[0].EnterSunRotation = new Vector3(90, -30, 0);
        States[0].ExitSunRotation = new Vector3(270, -30, 0);
        States[0].StateHoursCount = 16;

        States[1].State = DayState.Night;
        States[1].DayStateDurationInSecond = 20f;
        States[1].StateColor = new Color((float)41 / (float)255, (float)34 / (float)255, (float)13 / (float)255);
        States[1].EnterSunRotation = new Vector3(270, -30, 0);
        States[1].EnterSunRotation = new Vector3(90, -30, 0);
        States[1].StateHoursCount = 8;

        TotalDayTime = States[0].StateHoursCount + States[1].StateHoursCount;

        CurrentDayStateIndex = 1;
    }
    void ChangeDayState()
    {
        CurrentDayStateIndex++;
        if (CurrentDayStateIndex == States.Length)
        { 

        }
        CurrentDayStateIndex = (int)Mathf.Repeat(CurrentDayStateIndex, States.Length);
        States[CurrentDayStateIndex].TimeStateChanged = Time.time;
        DayStatesProperties NextState = States[(int)Mathf.Repeat(CurrentDayStateIndex + 1, States.Length)];
        onDayStateChanged?.Invoke(States[CurrentDayStateIndex], NextState);

        StartChrono(States[CurrentDayStateIndex].DayStateDurationInSecond, ChangeDayState, DayTimeCallback);
    }
    void DayTimeCallback(float timeElapsed, float timeRatio)
    {
        CurrentTimeOfDay = Mathf.Lerp(0, CurrentDayState.StateHoursCount, timeRatio);
    }
    #endregion
}
public class DayStateChangedEvent : UnityEvent<DayStatesProperties, DayStatesProperties> { }