using CZToolKit.Core.Singletons;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.Core
{
    public class AudioSystem : CZMonoSingleton<AudioSystem>
    {
        readonly GameObject parent;
        private List<AudioSource> audioSources = new List<AudioSource>();

        public AudioSource TakeAudioSource()
        {
            AudioSource audioSource = audioSources.Find(_audioSource => _audioSource.isPlaying == false);

            if (audioSource == null)
            {
                audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
                audioSource.transform.SetParent(transform);
                audioSources.Add(audioSource);
            }
            ResetAudioSource(audioSource);
            return audioSource;
        }

        public AudioSource Play2D(AudioClip _clip)
        {
            AudioSource audioSource = TakeAudioSource();
            audioSource.spatialBlend = 0;
            audioSource.clip = _clip;
            audioSource.Play();
            return audioSource;
        }

        public AudioSource Play3D(AudioClip _clip, Vector3 _position)
        {
            AudioSource audioSource = TakeAudioSource();
            audioSource.spatialBlend = 1;
            audioSource.clip = _clip;
            audioSource.transform.position = _position;
            audioSource.Play();
            return audioSource;
        }

        void ResetAudioSource(AudioSource _audioSource)
        {
            _audioSource.loop = false;
            _audioSource.playOnAwake = false;
            _audioSource.volume = 1;
            _audioSource.spatialBlend = 0;
        }
    }
}
