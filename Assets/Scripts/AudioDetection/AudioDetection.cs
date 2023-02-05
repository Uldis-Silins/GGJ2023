using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDetection : MonoBehaviour
{
    public int sampleWindow = 64;

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

        return totalLoudness;
    }
}
