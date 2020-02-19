using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Borodar.FarlandSkies.LowPoly;

public class GameManager : Singleton<GameManager>
{
    public GameData _data;
    public int FinalPhaseIndex;
    public float DayTimeTransitionDuration = 10f;
    public int DaysToSkipCount = 5;
    private Pedestals zonePedestals;
    private Tribe tribe;




    protected override void Awake()
    {
        base.Awake();
        onGamePaused = new GamePausedEvent();
    }

    protected override void Start()
    {
        base.Start();
        zonePedestals = ((GameObject) ObjectsManager.I["ZonePedestals"]).GetComponent<Pedestals>();
        tribe = ((GameObject) ObjectsManager.I["Tribe"]).GetComponent<Tribe>();
        zonePedestals.onGameFinished.AddListener(FinishGame);
        //InputManager.I.onPauseKeyPressed.AddListener(() => { PauseGame(!GamePaused); });
    }

    public void FinishGame()
    {
        tribe.StartHappy();
        StartCoroutine(GetComponent<AmbiantManager>().ModifyCycleProgress(FinalPhaseIndex, DayTimeTransitionDuration, DaysToSkipCount));
    }
}


