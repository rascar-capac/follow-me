using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneInteractable : BaseMonoBehaviour
{
    List<Zone> InZones;

    protected override void Start()
    {
        base.Start();
        InZones = new List<Zone>();
    }
    protected virtual void Update()
    {
        foreach (Zone z in InZones)
        {
            ApplyZoneEffect(z);
        }
    }

    public virtual void EnterZone(Zone zone)
    {
        InZones.Add(zone);
    }
    public virtual void ExitZone(Zone zone)
    {
        InZones.Remove(zone);
    }

    public virtual void ApplyZoneEffect(Zone zone) {; }
}
