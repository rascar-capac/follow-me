using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tribe : ZoneInteractable
{
    [Header("Current Tribe life.")]
    public float Life = 100.0f;
    [Header("Current Tribe Fuel.")]
    public float Fuel = 100.0f;


    public UnityEvent onTribeLifeEnterCritical = new UnityEvent();
    public UnityEvent onTribeLifeExitCritical = new UnityEvent();

    bool TribeLifeCritical = false;

    protected override void Start()
    {
        base.Start();
        Life = GameManager.I._data.InitialTribeLife;
        Fuel = GameManager.I._data.InitialTribeFuel;
    }

    protected override void Update()
    {
        base.Update();
        CriticalLife();
        UpdateFuel();
    }

    public void UpdateFuel()
    {
        Fuel -= GameManager.I._data.FuelLossSpeed * Time.deltaTime;
    }

    public void CriticalLife()
    {
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
        FuelZone(zone);
    }

    public void FuelZone(Zone zone)
    {
        Fuel -= zone.LooseFuelSpeed * Time.deltaTime;
        Fuel += zone.GainFuelSpeed * Time.deltaTime;
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
