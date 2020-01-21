using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : BaseMonoBehaviour
{
    [HideInInspector]
    public Transform playerBody;
    Transform Optimum;
    Renderer NeedleRenderer;
    GameObject Compass;

    [Header("The color of the compass when direction is correct")]
    public Material GoodDirection;
    [Header("The color of the compass when direction is NOT correct")]
    public Material BadDirection;
    [Header("The color of the compass when unavailable")]
    public Material UnavailableCompass;

    float xRotation = 0f;
    // Start is called before the first frame update
    protected override void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        playerBody = ((GameObject)ObjectsManager.I["Player"]).transform;
        Optimum = ((GameObject)ObjectsManager.I["Optimum"]).transform;
        Compass = GameObject.Find("Compass");
        NeedleRenderer = GameObject.Find("Needle").GetComponent<MeshRenderer>();
        NeedleRenderer.material = GoodDirection;
        transform.SetParent(playerBody);
        Compass.SetActive(GameManager.I._data.CompassActive);
        InputManager.I.onLookInputAxisEvent.AddListener(RotateCamera);
    }

    // Update is called once per frame
    void Update()
    {
        VerifyCompass();
    }

    void RotateCamera(InputAxisUnityEventArg axis)
    {
        float mouseX = axis.XValue * GameManager.I._data.mouseSensitivity * Time.deltaTime;
        float mouseY = axis.YValue * GameManager.I._data.mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void VerifyCompass()
    {
        if (!GameManager.I._data.CompassActive || GameManager.I._data.CompassUsable == null)
        {
            NeedleRenderer.material = UnavailableCompass;
            return;
        }


        int i = 0;
        for (i = 0; i < GameManager.I._data.CompassUsable.Count; i++)
        {
            if (GameManager.I._data.CompassUsable[i] == AmbiantManager.I.CurrentDayState.State)
                break;
        }

        if (i >= GameManager.I._data.CompassUsable.Count)
        {
            NeedleRenderer.material = UnavailableCompass;
            return;
        }

        Vector3 ray = new Vector3(Optimum.position.x, transform.position.y, Optimum.position.z) - transform.position;
        Vector3 rayProjected = Vector3.ProjectOnPlane(ray, Vector3.up);
        Vector3 forwardProjected = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        Debug.DrawLine(transform.position, Optimum.position);
        float angle = Vector3.Angle(rayProjected, forwardProjected);
        if (angle > GameManager.I._data.CompassThresshold || angle < -GameManager.I._data.CompassThresshold)
            NeedleRenderer.material = BadDirection;
        else
            NeedleRenderer.material = GoodDirection;
    }


}
