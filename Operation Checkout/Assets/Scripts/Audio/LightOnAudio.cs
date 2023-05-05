using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMG
{
    [RequireComponent(typeof(Light))]
    public class LightOnAudio : MonoBehaviour
    {
        public int band;
        public float _minIntensity, _maxIntensity;
        private Light _light;

        private void Awake()
        {
            _light = GetComponent<Light>();
        }

        private void Update()
        {
            _light.intensity = (AudioPeer._audioBandBuffer[band] * (_maxIntensity - _minIntensity) + _minIntensity);
        }
    }
}