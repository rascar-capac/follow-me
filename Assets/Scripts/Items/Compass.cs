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

        foreach (Quest q in QuestManager.I.Quests)
        {
            if (q.ItemsToActivate == null || q.ItemsToActivate.Count <= 0)
                continue;

            for (int i = 0; i < q.ItemsToActivate.Count; i++)
            {
                if (q.ItemsToActivate[i]._itemData.IsActivable)
                {
                    dist = Vector3.Distance(q.ItemsToActivate[i].transform.position, Needle.transform.position);
                    if (dist < min)
                    {
                        min = dist;
                        Nearest = q.ItemsToActivate[i];
                    }
                }
            }
        }
        if (Nearest)
        {
            Vector3 direction = Nearest.transform.position - Player.transform.position;
            float offsetAngle = Vector3.SignedAngle(- Player.transform.forward, Vector3.ProjectOnPlane(direction, Player.transform.up), Player.transform.up);
            Needle.transform.localRotation = Quaternion.Euler(0, offsetAngle, 0);
        }
    }
}
