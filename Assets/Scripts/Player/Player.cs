using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ZoneInteractable
{
    public float PlayerLifeStart = 100.0f;
    public float PlayerLife = 100.0f;

    PlayerLook playerLook;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        PlayerLife = PlayerLifeStart;
        playerLook = CameraManager.I._MainCamera.GetComponent<PlayerLook>();
    }


    public override void EnterZone(Zone zone)
    {
        playerLook.CompassActive = zone.AllowCompass;
    }
    public override void ExitZone(Zone zone)
    {
        playerLook.CompassActive = !zone.AllowCompass;
    }
}
