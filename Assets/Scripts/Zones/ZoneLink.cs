using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ZoneLink : Zone
{
    public int PhaseIndex;
    public int DayTimeTransitionDuration;
    public int DaysToSkipCount;
    public GameObject Tile;
    Player player;
    //Tribe tribe;
    AudioSource source;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        player.onZoneEnter.AddListener(EnteredZone);
        AmbiantManager.I.onTimePhaseChanged.AddListener(CheckPhase);
        //tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();
        source = GetComponent<AudioSource>();
        GameData gd = GameManager.I._data;
        Color tileColor = PhaseIndex == gd.Phases.Count ? gd.SpecialPhase.color : gd.Phases[PhaseIndex].color;
        Tile.GetComponent<Renderer>().material.SetColor("_Color", tileColor);
        Tile.GetComponent<Renderer>().material.SetFloat("_TileState", 0.1f);
    }

    public void EnteredZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this)
        {
            //tribe.StopAll();
            //tribe.SetMode(TribeEmotionMode.Happy);
            //tribe.StartRotating(LinkedZone.Egg.transform, speedMove: 200f);
            AmbiantManager.I.SkipDayTimeToPhase(PhaseIndex, DayTimeTransitionDuration, DaysToSkipCount);
            Tile.GetComponent<Renderer>().material.SetFloat("_TileState", 1f);
            source.PlayOneShot(SoundManager.I.TileactivationClips[Random.Range(0, SoundManager.I.TileactivationClips.Count)]);
            SoundManager.I.PlayTransitionWind();
            //source.clip = SoundManager.I.TileactivationClips[Random.Range(0, SoundManager.I.TileactivationClips.Count)];
            //source.loop = false;
            //source.Play();
        }
    }

    public void CheckPhase(int currentPhaseIndex)
    {
        if(currentPhaseIndex != PhaseIndex)
        {
            Tile.GetComponent<Renderer>().material.SetFloat("_TileState", 0.1f);
        }
    }
}
