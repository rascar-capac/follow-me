using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ZoneEvent : UnityEvent<ZoneInteractable, Zone> { }
public class ZoneInteractable : BaseMonoBehaviour
{
    [HideInInspector]
    public List<Zone> InZones;

    public ZoneEvent onZoneEnter = new ZoneEvent();
    public ZoneEvent onZoneExit = new ZoneEvent();

    public bool IsInZone(Zone zone)
    {
        return InZones.Exists(z => z == zone);
    }

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
        onZoneEnter.Invoke(this, zone);
    }
    public virtual void ExitZone(Zone zone)
    {
        InZones.Remove(zone);
        onZoneExit.Invoke(this, zone);
    }

    public virtual void ApplyZoneEffect(Zone zone) {; }
}
