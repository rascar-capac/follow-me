using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TribeDistanceMeter : Item
{
    public override void Init()
    {
        base.Init();
        UIManager.I.ShowTribeDistance(true);
    }

    private void OnDestroy()
    {
        if (UIManager.I)
            UIManager.I.ShowTribeDistance(false);
    }
}
