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
            CurrentPosition = Quaternion.AngleAxis((i+1) * AngleInterval, transform.up) * transform.forward * 3f;
            Instantiate(ToolItems[i].Prefab, CurrentPosition, Quaternion.identity, transform);
        }
    }

    private void Update()
    {
        //transform.eulerAngles.Set(transform.eulerAngles.x, transform.eulerAngles.y + 60f * Time.deltaTime, transform.eulerAngles.z);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 60f * Time.deltaTime, transform.eulerAngles.z);
        Debug.Log(transform.name);
    }
}
