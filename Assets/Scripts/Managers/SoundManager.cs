using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public List<AudioClip> CreatureClips;
    protected AudioSource CreatureSource;

    protected override void Start()
    {
        base.Start();
        CreatureSource = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<AudioSource>();
    }
    public void Play(string name)
    {
        AudioClip found = CreatureClips.Find(a => a.name == name);
        if (found)
        {
            CreatureSource.Stop();
            CreatureSource.PlayOneShot(found);
        }
    }
}
