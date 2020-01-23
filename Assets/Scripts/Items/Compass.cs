using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : Item
{
    public GameObject Needle;

    PlayerInventory player;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        if (!IsEnabled)
            return;

        float min = float.PositiveInfinity;
        float dist = 0.0f;
        int minIndex = int.MaxValue;
        
        for (int i = 0;i<player.dynamicItems.Count;i++)
        {
            dist = Vector3.Distance(player.dynamicItems[i].transform.position, Needle.transform.position);
            if (dist < min)
            {
                min = dist;
                minIndex = i;
            }
        }

        if (minIndex != int.MaxValue)
            Needle.transform.LookAt(player.dynamicItems[minIndex].transform);
    }
}
