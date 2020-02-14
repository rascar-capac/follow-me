using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Borodar.FarlandSkies.LowPoly;

public class GameManager : Singleton<GameManager>
{
    public GameData _data;
    public float FinalCycleEffectSpeed = 30f;
    public float FinalCycleEffectDuration = 10f;
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
        StartCoroutine(LaunchFinalCycleEffect());
    }

    public IEnumerator LaunchFinalCycleEffect()
    {
        float timer = FinalCycleEffectDuration;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            SkyboxCycleManager.Instance.CycleProgress += FinalCycleEffectSpeed * Time.deltaTime;
            SkyboxCycleManager.Instance.CycleProgress %= 100f;
            yield return null;
        }
    }
}


