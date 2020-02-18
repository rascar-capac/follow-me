using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public List<AudioClip> CreatureClips;
    protected AudioSource CreatureSource;

    public List<AudioClip> PedestalsClips;
    protected AudioSource PedestalsSource;

    public List<AudioClip> PlayerClips;
    public List<AudioClip> PlayerSteps;
    protected AudioSource PlayerSource;

    protected AudioSource AmbiantSource;
    public AudioClip GlobalAmbiance;


    protected override void Start()
    {
        base.Start();
        CreatureSource = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<AudioSource>();
        PedestalsSource = ((GameObject)ObjectsManager.I["ZonePedestals"]).GetComponent<AudioSource>();
        PlayerSource = ((GameObject)ObjectsManager.I["Player"]).GetComponent<AudioSource>();
        AmbiantSource = CameraManager.I._MainCamera.GetComponent<AudioSource>();
        StartCoroutine(PlayWalkSound());

    }


    public void PlayZoneAmbiance(AudioClip clip=null, bool loop = false)
    {
        if (clip == null)
            return;
        AmbiantSource.Stop();
        AmbiantSource.clip = clip;
        AmbiantSource.loop = loop;
        AmbiantSource.Play();
        StartChrono(clip.length, PlayGlobalAmbiance);
    }
    void PlayGlobalAmbiance()
    {
        AmbiantSource.Stop();
        AmbiantSource.clip = GlobalAmbiance;
        AmbiantSource.loop = true;
        AmbiantSource.Play();
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
    public void PlayPlayer(string name)
    {
        if (!PlayerSource)
            PlayerSource = ((GameObject)ObjectsManager.I["Player"]).GetComponent<AudioSource>();
        AudioClip found = PlayerClips.Find(a => a.name == name);
        if (found)
        {
            if (PlayerSource.isPlaying && PlayerSource.clip == found)
                return;
            StopPlayerSound();
            PlayerSource.clip = found;
            PlayerSource.loop = true;
            PlayerSource.Play();
        }
    }

    public bool IsWalking = false;
    IEnumerator PlayWalkSound()
    {
        if (!PlayerSource)
            PlayerSource = ((GameObject)ObjectsManager.I["Player"]).GetComponent<AudioSource>();
        while (true)
        {
            if (!IsWalking)
            {
                StopPlayerSound();

                while (!IsWalking)
                {
                    yield return null;
                }
            }

            AudioClip found = PlayerSteps[Random.Range(0, PlayerSteps.Count)];
            while (PlayerSource.clip == found)
            {
                found = PlayerSteps[Random.Range(0, PlayerSteps.Count)];
            }
            PlayerSource.clip = found;
            PlayerSource.loop = false;
            PlayerSource.pitch = Random.Range((float)-1, (float)1);
            PlayerSource.Play();

            yield return new WaitForSeconds(found.length * PlayerSource.pitch);
        }
    }

    public void StopPlayerSound()
    {
        PlayerSource.Stop();
    }
}
