using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHealth : MonoBehaviour
{
    [SerializeField] private Image m_healthImage;

    public Sprite[] healthAmountSprites;

    public void SetHealthImage(int health)
    {
        int index = healthAmountSprites.Length - health;
        m_healthImage.sprite = healthAmountSprites[index];
    }
}
