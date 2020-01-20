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


    public void LooseLife(Zone zone)
    {
        PlayerLife -= zone.HurtSpeed * Time.deltaTime;
    }

    public void GainLife(Zone zone)
    {
        PlayerLife += zone.GainSpeed * Time.deltaTime;
    }

    public void ActivateCompass(Zone zone)
    {
        playerLook.CompassActive = zone.AllowCompass;
    }

    public override void ApplyZoneEffect(Zone zone)
    {
        base.ApplyZoneEffect(zone);
        ActivateCompass(zone);
        LooseLife(zone);
        GainLife(zone);
    }
}
