using System;
using UnityEngine;

namespace AI
{
    [Serializable]
    public class AISoundChainElement
    {
        public AudioSource audioSource;
        public float delayBefore;
    }
}