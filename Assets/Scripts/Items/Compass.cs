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
        Item Nearest = null;

        foreach (Quest q in Player.Quests)
        {
            if (q.ItemsToActivate == null || q.ItemsToActivate.Count <= 0)
                continue;

            for (int i = 0; i < q.ItemsToActivate.Count; i++)
            {
                dist = Vector3.Distance(q.ItemsToActivate[i].transform.position, Needle.transform.position);
                if (dist < min)
                {
                    min = dist;
                    Nearest = q.ItemsToActivate[i];
                }
            }
        }
        if (Nearest)
            Needle.transform.LookAt(Vector3.ProjectOnPlane(Nearest.transform.position, Vector3.up));
    }
}
