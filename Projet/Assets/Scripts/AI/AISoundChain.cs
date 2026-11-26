using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace AI
{
    public class AISoundChain : MonoBehaviour
    {
        public bool playOnAwake;
        public bool loop;
        public List<AISoundChainElement> elements = new();
        
        private int _index;
        private bool _isPlaying;
        private bool _isWaitingToPlay;

        private void Start()
        {
            if (playOnAwake)
                Play();
        }

        private void Update()
        {
            if (_isWaitingToPlay || elements[_index].audioSource.isPlaying) return;
            
            _index++;
            if (_index < elements.Count)
            {
                StartCoroutine(Play(elements[_index].delayBefore));
                return;
            }
            
            _isPlaying = false;
            if (loop)
                Play();
        }

        public void Play()
        {
            if (_isPlaying) return;
            if (elements.Count <= 0) return;
            
            _isPlaying = true;
            _index = 0;

            StartCoroutine(Play(elements[_index].delayBefore));
        }

        private IEnumerator Play(float delay)
        {
            _isWaitingToPlay = true;
            yield return new WaitForSeconds(delay);

            _isWaitingToPlay = false;
            var element = elements[_index];
            element.audioSource.Play();
        }
    }
}