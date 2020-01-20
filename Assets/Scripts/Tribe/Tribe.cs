using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tribe : ZoneInteractable
{
    [Header("Current Tribe life.")]
    public float Life = 100.0f;

    public UnityEvent onTribeLifeEnterCritical = new UnityEvent();
    public UnityEvent onTribeLifeExitCritical = new UnityEvent();

    bool TribeLifeCritical = false;

    protected override void Start()
    {
        base.Start();
        Life = GameManager.I._data.InitialTribeLife;
    }


    protected override void Update()
    {
        base.Update();

        float TribeLifeRatio = Life / GameManager.I._data.InitialTribeLife;

        if (TribeLifeRatio <= GameManager.I._data.TribeLifeThresshold && !TribeLifeCritical)
        {
            TribeLifeCritical = true;
            onTribeLifeEnterCritical.Invoke();
        }
        else if (TribeLifeRatio > GameManager.I._data.TribeLifeThresshold && TribeLifeCritical)
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
