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
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        Vector3 ray = PromisedLand.position - transform.position;
        float angle = Vector3.SignedAngle(ray, transform.forward, Vector3.up);
        if (angle > CompassThresshold || angle < -CompassThresshold)
            NeedleRenderer.material = BadDirection;
        else
            NeedleRenderer.material = GoodDirection;
    }
}
