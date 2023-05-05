using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMG
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPeer : MonoBehaviour
    {
        private AudioSource _audioSource;
        public static float[] _samples = new float[512];
        private float[] _frequencyBand = new float[8];
        private float[] _bandBuffer = new float[8];
        private float[] _bufferDecrease = new float[8];

        private float[] _frequencyBandHighest = new float[8];
        public static float[] _audioBand = new float[8];
        public static float[] _audioBandBuffer = new float[8];

        public static float _amplitude, _amplitudeBuffer;
        private float _amplitudeHighest;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            GetSpectrumAudioSource();
            MakeFrequencyBands();
            BandBuffer();
            CreateAudioBands();
            GetAmplitude();
        }

        void GetAmplitude()
        {
            float _currentAmplitude = 0;
            float _currentAmplitudeBuffer = 0;
            for (int i = 0; i < 8; i++)
            {
                _currentAmplitude += _audioBand[i];
                _currentAmplitudeBuffer += _audioBandBuffer[i];
            }

            if (_currentAmplitude > _amplitudeHighest)
                _amplitudeHighest = _currentAmplitude;

            _amplitude = Mathf.Clamp01(_currentAmplitude / _amplitudeHighest);
            _amplitudeBuffer = Mathf.Clamp01(_currentAmplitudeBuffer / _amplitudeHighest);
        }

        void CreateAudioBands()
        {
            for (int i = 0; i < 8; i++)
            {
                if (_frequencyBand[i] > _frequencyBandHighest[i])
                {
                    _frequencyBandHighest[i] = _frequencyBand[i];
                }
                
                _audioBand[i] = (_frequencyBand[i] / _frequencyBandHighest[i]);
                _audioBandBuffer[i] = (_bandBuffer[i] / _frequencyBandHighest[i]);
            }
        }

        void GetSpectrumAudioSource()
        {
            _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
        }

        void BandBuffer()
        {
            for (int g = 0; g < 8; ++g)
            {
                if (_frequencyBand[g] > _bandBuffer[g])
                {
                    _bandBuffer[g] = _frequencyBand[g];
                    _bufferDecrease[g] = 0.005f;
                }

                if (_frequencyBand[g] < _bandBuffer[g])
                {
                    _bandBuffer[g] -= _bufferDecrease[g];
                    _bufferDecrease[g] *= 1.5f;
                }
            }
        }

        void MakeFrequencyBands()
        {
            /*
             * 20-60 Hz
             * 60-250 Hz
             * 250-500Hz
             * 500-2000 Hz
             * 2000-4000 Hz
             * 4000-6000 Hz
             * 6000-20000 Hz
             *
             * 0 - 2 = 86 Hz
             * 1 - 4
             * 2   8
             * 3   16
             * 4   32
             * 5   64 
             * 6   128 
             * 7   256
             *
             */

            int count = 0;

            for (int i = 1; i < 8; i++)
            {
                float average = 0;

                int sampleCount = (int) MathF.Pow(2, i);

                if (i - 1 == 7)
                {
                    sampleCount += 2;
                }

                for (int j = 0; j < sampleCount; j++)
                {
                    average += _samples[count] * (count + 1);
                    count++;
                }

                average /= sampleCount;

                _frequencyBand[i - 1] = average * 10;
            }
        }
    }
}