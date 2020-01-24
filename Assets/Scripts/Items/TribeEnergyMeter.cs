using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TribeEnergyMeter : Item
{
    Player Player;
    protected override void Start()
    {
        base.Start();
        Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        Player.onPlayerEnergyNullEnter.AddListener(() => { if (GameManager.I._data.TribeEnergyMeterEnergyLowUnusable) IsEnabled = false; });
        Player.onPlayerEnergyNullExit.AddListener(() => { if (GameManager.I._data.TribeEnergyMeterEnergyLowUnusable) IsEnabled = true; });

    }
    public override void Init()
    {
        base.Init();
        UIManager.I.ShowTribeEnergy(IsEnabled);
    }

    private void Update()
    {
        if (IsEnabled && AmbiantManager.I.IsUsableNow(GameManager.I._data.TribeEnergyMeterUsable))
            UIManager.I.SetTribeEnergy();
    }

    private void OnDestroy()
    {
        if (UIManager.I)
            UIManager.I.ShowTribeEnergy(false);
    }
}
