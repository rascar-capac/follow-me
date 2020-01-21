using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : ZoneInteractable
{
    [Header("Current player life")]
    public float PlayerLife = 100.0f;
    [Header("Current player oxygen")]
    public float PlayerOxygen = 100.0f;

    bool CriticalLife = false;
    PlayerLook playerLook;
    PlayerMovement playerMove;
    public UnityEvent onPlayerLifeEnterCritical = new UnityEvent();
    public UnityEvent onPlayerLifeExitCritical = new UnityEvent();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        PlayerLife = GameManager.I._data.InitialPlayerLife;
        PlayerOxygen = GameManager.I._data.InitialPlayerOxygen;
        playerLook = CameraManager.I._MainCamera.GetComponent<PlayerLook>();
        playerMove = GetComponent<PlayerMovement>();
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
        GameManager.I._data.CompassActive = zone.AllowCompass;
    }

    public override void ApplyZoneEffect(Zone zone)
    {
        base.ApplyZoneEffect(zone);
        ActivateCompass(zone);
        LooseLife(zone);
        GainLife(zone);
    }

    protected override void Update()
    {
        base.Update();
        LifeCritical();
        UpdateOxygen();
    }

    public void UpdateOxygen()
    {
        if (playerMove.IsRunning)
            PlayerOxygen -= GameManager.I._data.OxygenLossRun * Time.deltaTime;
        else
            PlayerOxygen -= GameManager.I._data.OxygenLossWalk * Time.deltaTime;

    }

    public void LifeCritical()
    {
        float lifeRatio = PlayerLife / GameManager.I._data.InitialPlayerLife;
        if (lifeRatio <= GameManager.I._data.PlayerLifeThresshold && !CriticalLife)
        {
            CriticalLife = true;
            onPlayerLifeEnterCritical.Invoke();
        }
        else if (lifeRatio > GameManager.I._data.PlayerLifeThresshold && CriticalLife)
        {
            CriticalLife = false;
            onPlayerLifeExitCritical.Invoke();
        }
    }
}
