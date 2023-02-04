using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHealth : MonoBehaviour
{
    [SerializeField] private Image m_healthImage;

    [SerializeField] private Sprite[] m_healthAmountSprites;

    public void SetHealthImage(int health)
    {
        health = Mathf.Clamp(health, 0, m_healthAmountSprites.Length - 1);
        m_healthImage.sprite = m_healthAmountSprites[health];
    }
}
