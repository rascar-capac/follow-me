using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ToolItem
{
    public bool Owned;
    public GameObject Prefab;
    public GameObject CreatedObject;
    public MeshRenderer renderer;
    public Color InitialColor;
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
            ToolItems[i].CreatedObject = Instantiate(ToolItems[i].Prefab, CurrentPosition, Quaternion.identity, transform);
            ToolItems[i].CreatedObject.transform.rotation = Quaternion.LookRotation(CameraManager.I._MainCamera.transform.up, - CameraManager.I._MainCamera.transform.forward);
            ToolItems[i].renderer = ToolItems[i].CreatedObject.GetComponentInChildren<MeshRenderer>();
            ToolItems[i].InitialColor = ToolItems[i].renderer.material.color;
        }
        InputManager.I.onLookInputAxisEvent.AddListener(test);

    }

    private void Update()
    {
        if (!UIManager.I.ToolsInvetoryOpened)
            return;

        for (int i = 0; i < ToolItems.Length; i++)
        {
            ToolItems[i].CreatedObject.transform.rotation = Quaternion.LookRotation(CameraManager.I._MainCamera.transform.up, -CameraManager.I._MainCamera.transform.forward);
        }
    }


    public void test(InputAxisUnityEventArg axis)
    {
        if (!UIManager.I.ToolsInvetoryOpened)
            return;
        Debug.Log($"x: " + axis.XValue + " y: " + axis.YValue);

        Vector3 forwardProjected = Vector3.ProjectOnPlane(transform.forward, transform.up);
        Vector3 axisProjected = Vector3.ProjectOnPlane(new Vector3(axis.XValue, axis.YValue, 0), transform.up);
        float angle = Vector3.SignedAngle(forwardProjected, axisProjected, transform.up);
        angle = Mathf.Repeat(angle, 360);
        int index = (int)(angle / AngleInterval);
        for (int i = 0; i < ToolItems.Length; i++)
        {
            if (i == index)
                ToolItems[i].CreatedObject.transform.GetChild(0).transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            else
                ToolItems[i].CreatedObject.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
