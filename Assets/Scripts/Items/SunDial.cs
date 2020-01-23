using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunDial : Item
{
    public override void Init()
    {
        base.Init();
        UIManager.I.ShowTime(true);
    }

    private void OnDestroy()
    {
        if (UIManager.I)
            UIManager.I.ShowTime(false);
    }
}
