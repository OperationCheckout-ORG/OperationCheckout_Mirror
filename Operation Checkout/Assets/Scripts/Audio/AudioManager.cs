using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public AudioClip[] sfxClips;
    public AudioClip[] musicClips;

    public bool randomizePitch = false;
    public float pitchRange = 0.1f;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Start () {
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySFX(int index) {
        if (index >= 0 && index < sfxClips.Length) {
            sfxSource.clip = sfxClips[index];
            if (randomizePitch) {
                sfxSource.pitch = Random.Range(1.0f - pitchRange, 1.0f + pitchRange);
            }
            sfxSource.Play();
        }
    }

    public void PlayMusic(int index) {
        if (index >= 0 && index < musicClips.Length) {
            musicSource.clip = musicClips[index];
            musicSource.Play();
        }
    }

    public void PlayRandomSFX() {
        int randomIndex = Random.Range(0, sfxClips.Length);
        PlaySFX(randomIndex);
    }

    public void PlayRandomMusic() {
        int randomIndex = Random.Range(0, musicClips.Length);
        PlayMusic(randomIndex);
    }
    
    public void StopSFXPlayback() {
        sfxSource.Stop();
    }

    public void StopMusicPlayback()
    {
        musicSource.Stop();
    }
}