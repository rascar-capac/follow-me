using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct ToolItem
{
    public bool Owned;
    public GameObject Prefab;
    public GameObject CreatedObject;
    public MeshRenderer renderer;
    public Color InitialColor;
}

public class ToolSelectedEvent : UnityEvent<GameObject, Hand> { }
public class ToolsInventoryManager : Singleton<ToolsInventoryManager>
{
    public ToolItem[] ToolItems;
    public float AngleInterval;
    public int CurrentIndex = 0;
    public ToolSelectedEvent onToolSelected = new ToolSelectedEvent();
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
        InputManager.I.onLookInputAxisEvent.AddListener(PointObject);
        InputManager.I.onPickUpKeyPressed.AddListener(SelectObjectLeft);
        InputManager.I.onUIInventoryKeyPressed.AddListener(SelectObjectRight);
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


    public void PointObject(InputAxisUnityEventArg axis)
    {
        if (!UIManager.I.ToolsInvetoryOpened)
            return;

        Vector3 vAxis = new Vector3(axis.XValue, axis.YValue, 0);
        if (vAxis.sqrMagnitude < 0.81f)
            return;

        float angle = Vector3.SignedAngle(Vector3.up, vAxis, -Vector3.forward);
        CurrentIndex = (int)(Mathf.Repeat(angle-(AngleInterval*0.5f), 360) / AngleInterval);

        for (int i = 0; i < ToolItems.Length; i++)
        {
            if (i == CurrentIndex)
                ToolItems[i].CreatedObject.transform.GetChild(0).transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            else
                ToolItems[i].CreatedObject.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void SelectObjectLeft()
    {
        if (!UIManager.I.ToolsInvetoryOpened)
            return;
        onToolSelected?.Invoke(ToolItems[CurrentIndex].Prefab, Hand.Left);
    }
    public void SelectObjectRight()
    {
        if (!UIManager.I.ToolsInvetoryOpened)
            return;
        onToolSelected?.Invoke(ToolItems[CurrentIndex].Prefab, Hand.Right);
    }
}
