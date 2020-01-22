using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ToolItem
{
    public bool Owned;
    public GameObject Prefab;
    public GameObject CreatedObject;
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
        }
        InputManager.I.onLookInputAxisEvent.AddListener(test);
    }

    private void Update()
    {
        for (int i = 0; i < ToolItems.Length; i++)
        {
            ToolItems[i].CreatedObject.transform.rotation = Quaternion.LookRotation(CameraManager.I._MainCamera.transform.up, -CameraManager.I._MainCamera.transform.forward);
        }

    }

    public void test(InputAxisUnityEventArg axis)
    {
        Debug.DrawRay(transform.position, (transform.right  * axis.XValue) + (transform.forward * axis.YValue) + (transform.up * transform.position.y)), Color.red);
        //if (axis.XValue < 0)
        //    transform.Rotate(Vector3.up, -2, Space.Self);
        //else if (axis.XValue > 0)
        //    transform.Rotate(Vector3.up, 2, Space.Self); 
    }
}
