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
}

public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        onGamePaused = new GamePausedEvent();
    }

    protected override void Start()
    {
        base.Start();
        InputManager.I.onPauseKeyPressed.AddListener(() => { PauseGame(!GamePaused); });
        InitDaysStates();
        ChangeDayState();
    }

    #region Day States Management
    public DayStateChangedEvent onDayStateChanged = new DayStateChangedEvent();
    public int CurrentDayStateIndex = 0;
    public DayStatesProperties[] States;
    void InitDaysStates()
    {
        States = new DayStatesProperties[((int)DayState._count)];

        States[0].State = DayState.Day;
        States[0].DayStateDurationInSecond = 2f;

        States[1].State = DayState.Night;
        States[1].DayStateDurationInSecond = 2f;

        CurrentDayStateIndex = -1;
    }
    void ChangeDayState()
    {
        CurrentDayStateIndex++;
        CurrentDayStateIndex = (int)Mathf.Repeat(CurrentDayStateIndex, States.Length);
        StartChrono(States[CurrentDayStateIndex].DayStateDurationInSecond, ChangeDayState);
        onDayStateChanged?.Invoke(States[CurrentDayStateIndex]);
    }
    #endregion
}

public class DayStateChangedEvent : UnityEvent<DayStatesProperties> { }

