using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneEgg : Zone
{
    public Item Egg;
    public GameObject Ray;
    public int PhaseIndex;
    public bool HasActivationAllowed = false;
    public bool IsTileActivated = false;
    public AudioClip ActivatedSound;
    public bool IsActivate => Ray.activeSelf;
    Player player;
    Tribe tribe;
    Renderer renderer;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        player.onZoneEnter.AddListener(EnteredZone);
        player.onZoneExit.AddListener(ExitedZone);
        AmbiantManager.I.onTimePhaseChanged.AddListener(AllowActivation);
        tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();
        GameData gd = GameManager.I._data;
        Color rayColor = PhaseIndex == gd.Phases.Count ? gd.SpecialPhase.color : gd.Phases[PhaseIndex].color;
        Ray.transform.GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_Color", rayColor);
        renderer = Egg.GetComponentInChildren<Renderer>();

    }

    protected void Update()
    {
        if (IsTileActivated && !IsActivate)
            renderer.material.SetFloat("_PulseState", Mathf.Sin(Time.time * 1.5f));
    }

    public void EnteredZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this && HasActivationAllowed && !IsActivate)
        {
            tribe.StopAll();
            Ray.gameObject.SetActive(true);
            if (ActivatedSound != null)
                GetComponent<AudioSource>().PlayOneShot(ActivatedSound);
            tribe.StartLive();
            Egg.ActivateItem();
        }
        else if (zone == this && !IsActivate)
        {
            tribe.StopAll();
            tribe.StartAggress();
        }
    }

    public void ExitedZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this && !HasActivationAllowed)
        {
            tribe.StopAll();
            tribe.StartLive();
        }
    }

    public void AllowActivation(int currentPhaseIndex)
    {
        HasActivationAllowed = currentPhaseIndex == PhaseIndex && IsTileActivated;
    }
}
