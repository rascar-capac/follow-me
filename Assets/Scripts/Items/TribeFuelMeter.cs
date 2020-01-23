using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TribeFuelMeter : Item
{
    public override void Init()
    {
        base.Init();
        UIManager.I.ShowTribeFuel(true);
    }

    private void OnDestroy()
    {
        if (UIManager.I)
            UIManager.I.ShowTribeFuel(false);
    }
}
