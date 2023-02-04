using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using KKSpeech;

public class InputController : MonoBehaviour
{
    [System.Serializable] public class ResultReceivedEvent : UnityEvent<string> { }

    [Space(20)]
    public ResultReceivedEvent onResultReceived;

    [SerializeField] private SpeechRecognizerListener m_speechListener;
    [SerializeField] private Button m_startRecordingButton;

    private string[] m_results;
    private const int RESULT_COUNT = 10;
    private int m_currentResultIndex;

    private void Awake()
    {
        m_results = new string[RESULT_COUNT];
        m_currentResultIndex = 0;
    }

    private void Start()
    {
        SpeechRecognizer.SetDetectionLanguage("en-US");

        if (SpeechRecognizer.ExistsOnDevice())
        {
            m_speechListener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
            m_speechListener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
            m_speechListener.onErrorDuringRecording.AddListener(OnError);
            m_speechListener.onErrorOnStartRecording.AddListener(OnError);
            m_speechListener.onFinalResults.AddListener(OnFinalResult);
            m_speechListener.onPartialResults.AddListener(OnPartialResult);
            m_speechListener.onEndOfSpeech.AddListener(OnEndOfSpeech);
            SpeechRecognizer.RequestAccess();
        }
        else
        {
            Debug.LogError("Sorry, but this device doesn't support speech recognition");
        }
    }

    public void OnFinalResult(string result)
    {
        Debug.Log(result);
        string[] split = result.Split(" ");
        result = split[0];
        onResultReceived.Invoke(result.ToLower());
        m_startRecordingButton.enabled = true;
    }

    public void OnPartialResult(string result)
    {
        onResultReceived.Invoke("Partial: " + result);
        string[] split = result.Split(" ");
        result = split[0];
        onResultReceived.Invoke(result.ToLower());
        Debug.Log("Partial: " + result);
    }

    public void OnAvailabilityChange(bool available)
    {
        m_startRecordingButton.enabled = available;

        if (!available)
        {
            Debug.LogError("Speech Recognition not available");
        }
    }

    public void OnAuthorizationStatusFetched(AuthorizationStatus status)
    {
        switch (status)
        {
            case AuthorizationStatus.Authorized:
                m_startRecordingButton.enabled = true;
                break;
            default:
                m_startRecordingButton.enabled = false;
                Debug.LogError("Cannot use Speech Recognition, authorization status is " + status);
                break;
        }
    }

    public void OnEndOfSpeech()
    {
        
    }

    public void OnError(string error)
    {
        Debug.LogError(error);

        m_startRecordingButton.enabled = true;
    }

    public void OnStartRecordingPressed()
    {
        if (SpeechRecognizer.IsRecording())
        {
#if UNITY_IOS && !UNITY_EDITOR
			SpeechRecognizer.StopIfRecording();
			m_startRecordingButton.enabled = false;
#elif UNITY_ANDROID && !UNITY_EDITOR
			SpeechRecognizer.StopIfRecording();
#endif
        }
        else
        {
            SpeechRecognizer.StartRecording(true);
        }
    }
}
