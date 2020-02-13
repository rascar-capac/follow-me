using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergyMeter : Item
{
    Player Player;

    protected override void Start()
    {
        base.Start();
        Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        Player.onPlayerEnergyNullEnter.AddListener(() => { if (GameManager.I._data.PlayerEnergyMeterEnergyLowUnusable) IsEnabled = false; });
        Player.onPlayerEnergyNullExit.AddListener(() => { if (GameManager.I._data.PlayerEnergyMeterEnergyLowUnusable) IsEnabled = true; });

    }
    public override void Init()
    {
        base.Init();
		//UIManager.I.ShowPlayerEnergy(IsEnabled);
    }

	private void Update()
	{
		//if (IsEnabled && AmbiantManager.I.IsUsableNow(GameManager.I._data.PlayerEnergyMeterUsable))
		//	UIManager.I.SetPlayerEnergy();
	}

	private void OnDestroy()
	{
		//if (UIManager.I)
		//	UIManager.I.ShowPlayerEnergy(false);
	}
}
