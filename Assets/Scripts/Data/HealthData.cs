using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Healt Data", menuName = "Data/Health")]
public class HealthData : ScriptableObject
{
    [System.Serializable]
    public class HealthSpriteData
    {
        public Sprite[] healthSprites;
    }

    public HealthSpriteData[] sprites;
}
