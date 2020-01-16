using System.Collections;
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
    public Color StateColor;
    public float TimeStateChanged;
}

public class AmbiantManager : Singleton<AmbiantManager>
{
    #region Fog
    public bool FogState = false;
    #endregion

    protected override void Start()
    {
        base.Start();
        InitDaysStates();
        ChangeDayState();
    }

    #region Day States Management
    public DayStateChangedEvent onDayStateChanged = new DayStateChangedEvent();
    public int CurrentDayStateIndex = 0;
    public DayStatesProperties[] States;
    public DayStatesProperties CurrentDayState => States[CurrentDayStateIndex];
    void InitDaysStates()
    {
        States = new DayStatesProperties[((int)DayState._count)];

        States[0].State = DayState.Day;
        States[0].DayStateDurationInSecond = 5f;
        States[0].StateColor = new Color((float)255 / (float)255, (float)240 / (float)255, (float)210 / (float)255);

        States[1].State = DayState.Night;
        States[1].DayStateDurationInSecond = 3f;
        States[1].StateColor = new Color((float)108 / (float)255, (float)91 / (float)255, (float)68 / (float)255);

        CurrentDayStateIndex = 1;
    }
    void ChangeDayState()
    {
        CurrentDayStateIndex++;
        CurrentDayStateIndex = (int)Mathf.Repeat(CurrentDayStateIndex, States.Length);
        States[CurrentDayStateIndex].TimeStateChanged = Time.time;
        DayStatesProperties NextState = States[(int)Mathf.Repeat(CurrentDayStateIndex + 1, States.Length)];
        onDayStateChanged?.Invoke(States[CurrentDayStateIndex], NextState);
        StartChrono(States[CurrentDayStateIndex].DayStateDurationInSecond, ChangeDayState);
    }
    #endregion
}
public class DayStateChangedEvent : UnityEvent<DayStatesProperties, DayStatesProperties> { }
