using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GreenNacho.AppManagement
{
    using Common;

    public enum MixerType
    {
        Sfx, Music, Count
    }
    
    public class AudioManager : MonoBehaviourSingleton<AudioManager>
    {
        [Header("Audio Mixers")]
        [SerializeField] AudioMixer[] audioMixers = new AudioMixer[(int)MixerType.Count];
        
        [Header("Audio Clips")]
        [SerializeField] AudioClip[] soundsUI = default;
        [SerializeField] AudioClip[] themes = default;
        
        [Header("Audio Sources")]
        [SerializeField] AudioSource sfxSource = default;
        [SerializeField] AudioSource musicSource = default;

        const float MixerMultiplier = 11.5f;
        const float MuteValue = -80f;

        Dictionary<MixerType, AudioMixer> audioMixersDic = new Dictionary<MixerType, AudioMixer>();

        void Start()
        {
            for (int i = 0; i < audioMixers.Length; i++)
                audioMixersDic.Add((MixerType)i, audioMixers[i]);
        }

        public void PlaySound(string soundName)
        {
            AudioClip audioClip = Array.Find(soundsUI, clip => clip.name == soundName);
            if (!audioClip)
            {
                Debug.LogError("There are no sounds named " + soundName + "registered in the Audio Manager.", gameObject);
                return;
            }
            sfxSource.clip = audioClip;
            sfxSource.Play();
        }

        public void PlayTheme(string themeName)
        {
            AudioClip audioClip = Array.Find(themes, theme => theme.name == themeName);
            if (!audioClip)
            {
                Debug.LogError("There are no themes named " + themeName + "registered in the Audio Manager.", gameObject);
                return;
            }
            musicSource.clip = audioClip;
            musicSource.Play();
        }

        public bool IsPlayingSound(string soundName)
        {
            return (sfxSource.isPlaying && sfxSource.clip.name == soundName);
        }
    }
}