using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SpeechToTextDebug : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_resultText;
    [SerializeField] private InputController m_inputController;

    private void Start()
    {
        m_inputController.onResultReceived.AddListener(HandleResultReceived);
    }

    private void HandleResultReceived(string result)
    {
        m_resultText.text = result;
    }
}
