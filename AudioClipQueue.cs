using System;
using System.Collections.Generic;
using UnityEngine;

namespace AudioClipQueue
{
    /// <summary>
    /// Play audio clips in queue
    /// </summary>
    public class AudioClipQueue : MonoBehaviour
    {
        [Space, SerializeField, Tooltip ("If true will start image gallery when enabled")]
        private bool loop;
        
        [SerializeField] private AudioSource attachedAudioSource;

        [SerializeField] private List <QueueingAudioClips> audioClips = new List <QueueingAudioClips> ();

        public List <QueueingAudioClips> AudioClips => audioClips;


        private int currentClipIndex;
    
        private QueueingAudioClips currentAudioClip;

        private bool stopPlaying;

        private void Start ()
        {
            attachedAudioSource.enabled = false;
            StartQueueing ();
        }

        private void Update ()
        {
            //when audiosource is playing don't do anything
            if (attachedAudioSource.isPlaying)
                return;
        
            if(stopPlaying)
            {
                StopPlaying ();
                return;
            }
            //If current index is last element in list play the clip and then mark the queue to be disabled;
            if (currentClipIndex == audioClips.Count-1)
            {
                attachedAudioSource.clip = audioClips [audioClips.Count-1].clip;
                attachedAudioSource.PlayDelayed (audioClips [audioClips.Count-1].delay);

                if(loop)
                {
                    currentClipIndex = 0;
                }
                else
                {
                    stopPlaying = true;
                }

                return;
            }

        
            //When no audioclip is playing move to next clip in list
            var q = audioClips [currentClipIndex++];

            attachedAudioSource.clip = q.clip;
            attachedAudioSource.PlayDelayed(q.delay);
        }

        private void StartQueueing ()
        {
            MoveNext (0);
            attachedAudioSource.enabled = true;
        }

        /// <summary>
        /// Move to next audioclip in queue
        /// </summary>
        /// <param name="index"></param>
        private void MoveNext (int index)
        {
            currentClipIndex = index;
            currentAudioClip = audioClips [currentClipIndex];

            attachedAudioSource.clip = currentAudioClip.clip;
        }

        /// <summary>
        /// Stop queueing audios. Set index back to 0 and disable this and audiosource components
        /// </summary>
        private void StopPlaying ()
        {
            enabled = false;
            attachedAudioSource.enabled = false;
            currentClipIndex = 0;
        }


    
        /// <summary>
        /// Editor friendly solution to easily modify audioclips and their delay
        /// </summary>
        [Serializable]
        public struct QueueingAudioClips
        {
            public AudioClip clip;
            public ulong delay;
        }
    }
}