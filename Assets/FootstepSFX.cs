using System;
using System.Collections.Generic;
using UnityEngine;

namespace solobranch.ggj2025
{
    public class FootstepSFX : MonoBehaviour
    {
        private List<AudioSource> audioSources = new();

        private void Start()
        {
            //initialize all audiosources into the list
            foreach (var audioSource in GetComponents<AudioSource>())
            {
                audioSources.Add(audioSource);
            }
        }

        public void PlayFootstepSound()
        {
            if (audioSources.Count == 0)
            {
                return;
            }

            //loop through all audiosources and toggle play or pause
            foreach (var audioSource in audioSources)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            
        }
        
        public void PauseFootstepSound()
        {
            if (audioSources.Count == 0)
            {
                return;
            }

            //loop through all audiosources and toggle play or pause
            foreach (var audioSource in audioSources)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Pause();
                }
            }
        }
    }
}
