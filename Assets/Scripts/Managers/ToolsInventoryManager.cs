using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ToolItem
{
    public bool Owned;
    public GameObject Prefab;
}

public class ToolsInventoryManager : Singleton<ToolsInventoryManager>
{
    public ToolItem[] ToolItems;

    public float AngleInterval;

    protected override void Start()
    {
        base.Start();
        AngleInterval = 360 / ToolItems.Length;

        Vector3 CurrentPosition = -transform.forward;

        for (int i = 0; i < ToolItems.Length; i++)
        { 
            CurrentPosition = transform.position + Quaternion.AngleAxis((i+1) * AngleInterval, transform.up) * transform.forward * 4f;
			Instantiate(ToolItems[i].Prefab, CurrentPosition, Quaternion.identity, transform);
		}
    }

    private void Update()
    {
		transform.Rotate(Vector3.up, 3, Space.Self); // Remplacer le 3

        Debug.Log(transform.name);
    }
}
