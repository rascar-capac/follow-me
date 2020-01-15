using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    [HideInInspector]
    public Transform playerBody;
    Transform PromisedLand;
    Renderer NeedleRenderer;
    public Material GoodDirection;
    public Material BadDirection;
    public float CompassThresshold = 15f;

    float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerBody = ((GameObject)ObjectsManager.I["Player"]).transform;
        PromisedLand = ((GameObject)ObjectsManager.I["PromisedLand"]).transform;
        NeedleRenderer = GameObject.Find("Needle").GetComponent<MeshRenderer>();
        NeedleRenderer.material = GoodDirection;
        transform.SetParent(playerBody);
    }

    // Update is called once per frame
    void Update()
    {
        RotateCamera();
        VerifyCompass();
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void VerifyCompass()
    {
        Vector3 ray = new Vector3(PromisedLand.position.x, transform.position.y, PromisedLand.position.z) - transform.position;
        Vector3 rayProjected = Vector3.ProjectOnPlane(ray, Vector3.up);
        Vector3 forwardProjected = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

        float angle = Vector3.Angle(rayProjected, forwardProjected);
        if (angle > CompassThresshold || angle < -CompassThresshold)
            NeedleRenderer.material = BadDirection;
        else
            NeedleRenderer.material = GoodDirection;
    }
}
