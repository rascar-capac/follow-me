using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ZoneLink : Zone
{
    public int PhaseIndex;
    public int DayTimeTransitionDuration;
    public int DaysToSkipCount;
    Player player;
    //Tribe tribe;
    AudioSource source;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        player.onZoneEnter.AddListener(EnteredZone);
        //tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();
        source = GetComponent<AudioSource>();
    }

    public void EnteredZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this)
        {
            //tribe.StopAll();
            //tribe.SetMode(TribeEmotionMode.Happy);
            //tribe.StartRotating(LinkedZone.Egg.transform, speedMove: 200f);
            AmbiantManager.I.SkipDayTimeToPhase(PhaseIndex, DayTimeTransitionDuration, DaysToSkipCount);
            source.PlayOneShot(SoundManager.I.TileactivationClips[Random.Range(0, SoundManager.I.TileactivationClips.Count)]);
            //source.clip = SoundManager.I.TileactivationClips[Random.Range(0, SoundManager.I.TileactivationClips.Count)];
            //source.loop = false;
            //source.Play();
        }
    }
}
