using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Borodar.FarlandSkies.LowPoly;

public class GameManager : Singleton<GameManager>
{
    public GameData _data;
    public float FinalCycleEffectDuration = 10f;
    public AnimationCurve FinalCycleEffectSpeedCurve;
    public float FinalCycleEffectMaxProgressPerSecond;
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
        float initialCycleProgress = SkyboxCycleManager.Instance.CycleProgress;
        while(timer >= 0)
        {
            timer -= Time.deltaTime;
            SkyboxCycleManager.Instance.CycleProgress += FinalCycleEffectSpeedCurve.Evaluate(timer / FinalCycleEffectDuration) * FinalCycleEffectMaxProgressPerSecond * Time.deltaTime;
            SkyboxCycleManager.Instance.CycleProgress %= 100f;
            yield return null;
        }
    }
}


