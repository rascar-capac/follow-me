using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : Item
{
    public GameObject Needle;

    PlayerInventory PlayerInventory;
    Player Player;

    protected override void Start()
    {
        base.Start();
        PlayerInventory = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerInventory>();
        Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        Player.onPlayerEnergyNullEnter.AddListener(() => { if (GameManager.I._data.CompassEnergyLowUnusable) IsEnabled = false; });
        Player.onPlayerEnergyNullExit.AddListener(() => { if (GameManager.I._data.CompassEnergyLowUnusable) IsEnabled = true; });
    }

    private void Update()
    {
        if (!IsEnabled)
            return;

        if (!AmbiantManager.I.IsUsableNow(GameManager.I._data.CompassUsable))
            return;

        float min = float.PositiveInfinity;
        float dist = 0.0f;
        int minIndex = int.MaxValue;
        
        for (int i = 0;i<PlayerInventory.dynamicItems.Count;i++)
        {
            dist = Vector3.Distance(PlayerInventory.dynamicItems[i].transform.position, Needle.transform.position);
            if (dist < min)
            {
                min = dist;
                minIndex = i;
            }
        }

        if (minIndex != int.MaxValue)
            Needle.transform.LookAt(Vector3.ProjectOnPlane(PlayerInventory.dynamicItems[minIndex].transform.position, Vector3.up));
    }
}
