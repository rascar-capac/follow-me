using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunDial : Item
{
    Player Player;

    protected override void Start()
    {
        base.Start();
        Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        Player.onPlayerEnergyNullEnter.AddListener(() => { if (GameManager.I._data.SundialEnergyLowUnusable) IsEnabled = false; });
        Player.onPlayerEnergyNullExit.AddListener(() => { if (GameManager.I._data.SundialEnergyLowUnusable) IsEnabled = true; });

    }
    public override void Init()
    {
        base.Init();
        //UIManager.I.ShowTime(IsEnabled);
    }

    private void Update()
    {
        //if (IsEnabled && AmbiantManager.I.IsUsableNow(GameManager.I._data.SundialUsable))
        //    UIManager.I.SetTimeOfDay();
    }
}
