using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ZoneInteractable
{
    public float PlayerLifeStart = 100.0f;
    public float PlayerLife = 100.0f;

    PlayerLook playerLook;
    float HurtingSpeed = 0.0f;
    float GainSpeed = 0.0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        PlayerLife = PlayerLifeStart;
        playerLook = CameraManager.I._MainCamera.GetComponent<PlayerLook>();
    }

    protected void Update()
    {
        LooseLife();
        GainLife();
    }

    public void LooseLife()
    {
        PlayerLife -= HurtingSpeed * Time.deltaTime;
    }

    public void GainLife()
    {
        PlayerLife += GainSpeed * Time.deltaTime;
    }

    public override void EnterZone(Zone zone)
    {
        playerLook.CompassActive = zone.AllowCompass;
        HurtingSpeed += zone.HurtSpeed;
        GainSpeed += zone.GainSpeed;
    }
    public override void ExitZone(Zone zone)
    {
        playerLook.CompassActive = !zone.AllowCompass;
        HurtingSpeed -= zone.HurtSpeed;
        GainSpeed -= zone.GainSpeed;
    }
}
