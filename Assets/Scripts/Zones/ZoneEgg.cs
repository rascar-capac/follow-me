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
    AudioSource zonesource;

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
        zonesource = GetComponent<AudioSource>();
    }

    protected void Update()
    {
        if (IsTileActivated && !IsActivate)
            renderer.material.SetFloat("_PulseState", Mathf.Sin(Time.time * 1.5f));

        zonesource.volume = Mathf.Clamp(1- Mathf.Lerp(0, 1, Vector3.Distance(Vector3.ProjectOnPlane(player.transform.position, Vector3.up), Vector3.ProjectOnPlane(transform.position, Vector3.up))/ zonesource.maxDistance), 0, 1);
    }

    public void EnteredZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this && HasActivationAllowed && !IsActivate)
        {
            AudioSource zonesource = GetComponent<AudioSource>();
            tribe.StopAll();
            Ray.gameObject.SetActive(true);
            if (ActivatedSound != null)
                zonesource.PlayOneShot(SoundManager.I.StonesClips[Random.Range(0, SoundManager.I.StonesClips.Count)]);
            tribe.StartLive();
            Egg.ActivateItem();
            StartChrono(2, () => {
                zonesource.clip = SoundManager.I.RaysClips[Random.Range(0, SoundManager.I.RaysClips.Count)];
                zonesource.loop = true;
                zonesource.Play();
            });
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
