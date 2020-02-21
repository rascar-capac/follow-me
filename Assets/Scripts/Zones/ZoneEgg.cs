using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneEgg : Zone
{
    public Item Egg;
    public ItemData PedestalStone;
    public GameObject Ray;
    public int PhaseIndex;
    public bool IsActivable = false;
    public bool IsActivated => Ray.activeSelf;
    public AudioClip ActivatedSound;
    public float PulsationSpeedThreshold1 = 180;
    public float PulsationSpeedThreshold2 = 100;
    Player player;
    Tribe tribe;
    Pedestals pedestals;
    Renderer eggRenderer;
    AudioSource zonesource;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        player.onZoneEnter.AddListener(EnteredZone);
        player.onZoneExit.AddListener(ExitedZone);
        AmbiantManager.I.onTimePhaseChanged.AddListener(CheckPhase);
        tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();
        pedestals = ((GameObject)ObjectsManager.I["ZonePedestals"]).GetComponent<Pedestals>();
        GameData gd = GameManager.I._data;
        Color rayColor = PhaseIndex == gd.Phases.Count ? gd.SpecialPhase.color : gd.Phases[PhaseIndex].color;
        Ray.transform.GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_Color", rayColor);
        eggRenderer = Egg.GetComponentInChildren<Renderer>();
        zonesource = GetComponent<AudioSource>();
    }

    protected void Update()
    {
        if (!IsActivated)
        {
            if(IsActivable)
            {
                float playerDistance = Vector3.Distance(Vector3.ProjectOnPlane(player.transform.position, Vector3.up), Vector3.ProjectOnPlane(transform.position, Vector3.up));
                float speed;
                if(playerDistance > PulsationSpeedThreshold1)
                {
                    speed = 2;
                }
                else if(playerDistance > PulsationSpeedThreshold2)
                {
                    speed = 4;
                }
                else
                {
                    speed = 8;
                }
                ChangePulseState(0.40f + Mathf.Sin(Time.time * speed) * 0.40f);
            }
        }

        // zonesource.volume = Mathf.Clamp(1- Mathf.Lerp(0, 1, Vector3.Distance(Vector3.ProjectOnPlane(player.transform.position, Vector3.up), Vector3.ProjectOnPlane(transform.position, Vector3.up))/ zonesource.maxDistance), 0, 1);
    }

    public void EnteredZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this && IsActivable && !IsActivated)
        {
            AudioSource zonesource = GetComponent<AudioSource>();
            tribe.StopAll();
            Ray.gameObject.SetActive(true);
            zonesource.clip = SoundManager.I.RaysClips[Random.Range(0, SoundManager.I.RaysClips.Count)];
            zonesource.loop = true;
            zonesource.Play();
            if (ActivatedSound != null)
                zonesource.PlayOneShot(SoundManager.I.StonesClips[Random.Range(0, SoundManager.I.StonesClips.Count)]);
            tribe.StartLive();
            //Egg.ActivateItem();
            ChangePulseState(0.8f);
            foreach(PedestalStoneMatch match in pedestals.PedestalStoneMatches)
            {
                Transform currentStone = match.Pedestal.transform.GetChild(0);
                if(PedestalStone == currentStone.GetComponent<Item>()._itemData)
                {
                    pedestals.ChangePulseState(0.8f, currentStone.GetComponentInChildren<Renderer>());
                }
            }
        }
        else if (zone == this && !IsActivated)
        {
            tribe.StopAll();
            tribe.StartAggress();
        }
    }

    public void ExitedZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this && !IsActivable)
        {
            tribe.StopAll();
            tribe.StartLive();
        }
    }

    public void CheckPhase(int currentPhaseIndex)
    {
        if(currentPhaseIndex == PhaseIndex)
        {
            IsActivable = true;
        }
        else
        {
            if(IsActivable && !IsActivated)
            {
                ChangePulseState(0.1f);
                IsActivable = false;
            }
        }
    }

    public void ChangePulseState(float value)
    {
        eggRenderer.material.SetFloat("_PulseState", value);
    }
}
