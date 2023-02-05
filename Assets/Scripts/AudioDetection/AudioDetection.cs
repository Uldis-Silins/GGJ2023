using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioDetection : MonoBehaviour
{
    [System.Serializable]
    public class AudioData
    {
        public bool isLow = true;
        public float targetLoudness = 0.5f;
        public float targetTime = 1f;
        public bool playerAttack;
    }

    public int sampleWindow = 64;
    public AudioSource micSource;

    public GameController gameController;

    public Image lowBar, hiBar;

    private AudioClip m_microphoneClip;

    private float[] m_samples = new float[512];
    private float[] m_freqBands = new float[8];

    private float m_lowAmount;
    private float m_hiAmount;

    [SerializeField] private TextMeshProUGUI m_freqText;
    [SerializeField] private AudioData[] m_data;

    private AudioData m_currentData;
    private int m_curIndex = 0;
    private float m_currentTimer, m_targetReachedTimer;

    private float m_targetReachedTime = 1;

    public AudioData CurrentData { get { return m_currentData; } }

    private void Start()
    {
        GetMicrophoneAudio();
        m_currentData = m_data[m_curIndex];
    }

    private void Update()
    {
        GetSpectrum();

        float low = m_freqBands[0] + m_freqBands[1] + m_freqBands[2];
        float hi = m_freqBands[3] + m_freqBands[4] + m_freqBands[5] + m_freqBands[6] + m_freqBands[7];

        low /= 3;
        hi /= 5;

        lowBar.fillAmount = m_currentData.isLow ? low : 0;
        hiBar.fillAmount = !m_currentData.isLow ? hi : 0;

        m_freqText.text = m_currentData.isLow ? "LO" : "HI"; 

        if (m_currentData.isLow)
        {
            if (low > m_currentData.targetLoudness)
            {
                lowBar.color = Color.green;
                m_currentTimer += Time.deltaTime;
            }
            else
            {
                lowBar.color = Color.red;
                m_currentTimer = 0f;
            }
        }
        else
        {
            if (hi > m_currentData.targetLoudness)
            {
                hiBar.color = Color.green;
                m_currentTimer += Time.deltaTime;
            }
            else
            {
                hiBar.color = Color.red;
                m_currentTimer = 0f;
            }
        }

        if(m_targetReachedTimer > m_targetReachedTime || m_currentTimer > m_currentData.targetTime)
        {
            m_curIndex++;

            if(m_targetReachedTimer > m_targetReachedTime && m_currentTimer < m_currentData.targetTime)
            {
                // Chic attack
                if(m_currentData.playerAttack)
                {
                    gameController.ChickAttack();
                }
                else
                {
                    gameController.ChickBlock();
                }
            }
            else
            {
                if (m_currentData.playerAttack)
                {
                    gameController.GopBlock();
                }
                else
                {
                    gameController.GopAttack();
                }
            }

            if(m_curIndex > 0 && m_curIndex % m_data.Length == 0)
            {
                m_targetReachedTime++;
            }

            m_currentData = m_data[m_curIndex % m_data.Length];
            m_currentData.playerAttack = Random.value > 0.5f ? false : true;
            m_currentTimer = 0f;
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
