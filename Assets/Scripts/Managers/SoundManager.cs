using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public List<AudioClip> CreatureClips;
    protected AudioSource CreatureSource;

    public List<AudioClip> PedestalsClips;
    protected AudioSource PedestalsSource;

    protected override void Start()
    {
        base.Start();
        CreatureSource = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<AudioSource>();
        PedestalsSource = ((GameObject)ObjectsManager.I["ZonePedestals"]).GetComponent<AudioSource>();
    }
    public void PlayCreature(string name)
    {
        if (!CreatureSource)
            CreatureSource = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<AudioSource>();

        AudioClip found = CreatureClips.Find(a => a.name == name);
        if (found)
        {
            CreatureSource.Stop();
            CreatureSource.PlayOneShot(found);
        }
    }
    public void PlayPedestals(string name)
    {
        if (!PedestalsSource)
            PedestalsSource = ((GameObject)ObjectsManager.I["ZonePedestals"]).GetComponent<AudioSource>();

        AudioClip found = PedestalsClips.Find(a => a.name == name);
        if (found)
        {
            PedestalsSource.Stop();
            PedestalsSource.PlayOneShot(found);
        }
    }
}
