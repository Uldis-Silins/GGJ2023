using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDetection : MonoBehaviour
{
    public int sampleWindow = 64;
    public AudioSource micSource;

    private AudioClip m_microphoneClip;

    private float[] m_samples = new float[512];

    private void Start()
    {
        GetMicrophoneAudio();
    }

    public void GetMicrophoneAudio()
    {
        string microphoneName = Microphone.devices[0];
        m_microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);
        micSource.clip = m_microphoneClip;
    }

    public float GetLoudnessFromMicrophone()
    {
        return GetLoudness(m_microphoneClip, Microphone.GetPosition(Microphone.devices[0]));
    }

    public void GetSpectrum()
    {
        micSource.GetSpectrumData(m_samples, 0, FFTWindow.Blackman)
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
