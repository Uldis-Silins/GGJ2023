using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioDetection : MonoBehaviour
{
    public int sampleWindow = 64;
    public AudioSource micSource;

    public Image[] loudnessBars;

    private AudioClip m_microphoneClip;

    private float[] m_samples = new float[512];
    private float[] m_freqBands = new float[8];

    private void Start()
    {
        GetMicrophoneAudio();
    }

    private void Update()
    {
        GetSpectrum();

        for (int i = 0; i < m_freqBands.Length; i++)
        {
            loudnessBars[i].fillAmount = m_freqBands[i];
        }
    }

    public void GetMicrophoneAudio()
    {
        string microphoneName = Microphone.devices[0];
        m_microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);
        micSource.clip = m_microphoneClip;
        micSource.Play();
        micSource.loop = true;
    }

    public float GetLoudnessFromMicrophone()
    {
        return GetLoudness(m_microphoneClip, Microphone.GetPosition(Microphone.devices[0]));
    }

    public void GetSpectrum()
    {
        micSource.GetSpectrumData(m_samples, 0, FFTWindow.Blackman);

        int count = 0;

        for (int i = 0; i < m_freqBands.Length; i++)
        {
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7) sampleCount += 2;

            float avarage = 0f;

            for (int j = 0; j < sampleCount; j++)
            {
                avarage += m_samples[count] * (count + 1);
                count++;
            }

            avarage /= count;

            m_freqBands[i] = avarage * 3;
        }
    }

    public float GetLoudness(AudioClip clip, int position)
    {
        int startPosition = position - sampleWindow;
        float[] sampleData = new float[sampleWindow];
        clip.GetData(sampleData, startPosition);

        float totalLoudness = 0;

        for (int i = 0; i < sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(sampleData[i]);
        }

        return totalLoudness / sampleWindow;
    }
}
