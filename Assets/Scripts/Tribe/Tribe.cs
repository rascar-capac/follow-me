using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tribe : ZoneInteractable
{
    [Header("Initial Tribe life when starting game.")]
    public float InitialLife = 100.0f;
    [Header("Current Tribe life.")]
    public float Life = 100.0f;
    [Header("Alert player when life is under threshold in ratio (between 0 and 1).")]
    public float CriticalLife = 0.1f;

    public UnityEvent onTribeLifeEnterCritical = new UnityEvent();
    public UnityEvent onTribeLifeExitCritical = new UnityEvent();

    bool TribeLifeCritical = false;

    protected override void Start()
    {
        base.Start();
        Life = InitialLife;
    }


    protected override void Update()
    {
        base.Update();

        float TribeLifeRatio = Life / InitialLife;

        if (TribeLifeRatio <= CriticalLife && !TribeLifeCritical)
        {
            TribeLifeCritical = true;
            onTribeLifeEnterCritical.Invoke();
        }
        else if (TribeLifeRatio > CriticalLife && TribeLifeCritical)
        {
            TribeLifeCritical = false;
            onTribeLifeExitCritical.Invoke();
        }
    }

    public override void ApplyZoneEffect(Zone zone)
    {
        base.ApplyZoneEffect(zone);
        LooseLife(zone);
        GainLife(zone);
    }

    public void LooseLife(Zone zone)
    {
        Life -= zone.HurtSpeed * Time.deltaTime;
    }

    public void GainLife(Zone zone)
    {
        Life += zone.GainSpeed * Time.deltaTime;
    }
}
