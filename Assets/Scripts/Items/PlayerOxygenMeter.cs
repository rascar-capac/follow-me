using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOxygenMeter : Item
{
    public override void Init()
    {
        base.Init();
        UIManager.I.ShowPlayerOxygen(true);
    }

    private void OnDestroy()
    {
        if (UIManager.I)
            UIManager.I.ShowPlayerOxygen(false);
    }
}
