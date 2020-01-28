using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : BaseMonoBehaviour
{
    [HideInInspector]
    public Transform playerBody;
    bool AllowLook = true;

    float xRotation = 0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        playerBody = ((GameObject)ObjectsManager.I["Player"]).transform;
        transform.SetParent(playerBody);
        InputManager.I.onLookInputAxisEvent.AddListener(RotateCamera);
        UIManager.I.onToolsInventoryOpenedEvent.AddListener((hand) => { AllowLook = false; });
        UIManager.I.onToolsInventoryClosedEvent.AddListener((hand) => { AllowLook = true; });
    }

    void RotateCamera(InputAxisUnityEventArg axis)
    {
        if (!AllowLook)
            return;

        float mouseX = axis.XValue * GameManager.I._data.CameraRotationSpeed * Time.deltaTime;
        float mouseY = axis.YValue * GameManager.I._data.CameraRotationSpeed * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

}
