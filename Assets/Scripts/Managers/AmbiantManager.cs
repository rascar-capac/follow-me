using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class AmbiantManager : Singleton<AmbiantManager>
{
    #region Fog
    GameObject Fog;
    Zone FogZone;
    Player Player;
    ParticleSystem FogParticles;


    void StartFog()
    {
        if (GameManager.I._data.MinimumTimeBetweenFog == 0)
            return;
        Fog.SetActive(true);
        Fog.transform.position = new Vector3(CameraManager.I._MainCamera.transform.position.x, CameraManager.I._MainCamera.transform.position.y, CameraManager.I._MainCamera.transform.position.z);// + Vector3.ProjectOnPlane(Player.transform.forward, Vector3.up);
        FogParticles.Play();
        StartChrono(GameManager.I._data.MinimumTimeBetweenFog, EndFog);
        Debug.Log("Fog on");
    }
    void EndFog()
    {
        if (GameManager.I._data.MinimumTimeBetweenFog == 0)
            return;
        FogParticles.Stop();
        Fog.SetActive(false) ;
        Player.ExitZone(FogZone);
        StartChrono(GameManager.I._data.MinimumTimeBetweenFog, StartFog);
        Debug.Log("Fog off");
    }
    #endregion

    protected override void Start()
    {
        base.Start();
        Fog = (GameObject)ObjectsManager.I["Fog"];
        FogParticles = Fog.GetComponentInChildren<ParticleSystem>();
        Fog.SetActive(false);
        FogZone = Fog.GetComponentInChildren<Zone>();
        Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        InitDaysStates();
        ChangeDayState();
        StartChrono(GameManager.I._data.MinimumTimeBetweenFog, StartFog);
    }

    #region Day States Management
    public DayStateChangedEvent onDayStateChanged = new DayStateChangedEvent();
    public int CurrentDayStateIndex = 0;
    public DayStatesProperties CurrentDayState => GameManager.I._data.States[CurrentDayStateIndex];
    public DayStatesProperties NextDayState => GameManager.I._data.States[(int)Mathf.Repeat(CurrentDayStateIndex + 1, GameManager.I._data.States.Length)];
    public bool IsDay => CurrentDayState.State == DayState.Day;
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
