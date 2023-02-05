using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    [System.Serializable]
    public class StateObjects
    {
        public StateType type;
        public GameObject[] objects;

        public void ToggleState(bool isEnabled)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(isEnabled);
            }
        }
    }

    public enum StateType { None, Comic, CharSelect, Fight, Spectate, GameOver }

    private delegate void StateHandler();

    private StateHandler m_currentState;

    [SerializeField] private StateObjects[] m_stateObjects;
    [SerializeField] private UI_PlayerHealth m_playerHealth, m_opponentHealth;
    [SerializeField] private HealthData m_healthData;
    [SerializeField] private UI_ComicStripController m_comicStripController;

    private Dictionary<StateType, StateHandler> m_stateHandlers;

    public StateType CurrentState { get; private set; }
    public bool IsInitialized { get { return m_currentState != null; } }

    private void Awake()
    {
        m_stateHandlers = new Dictionary<StateType, StateHandler>();
        m_stateHandlers.Add(StateType.None, null);
        m_stateHandlers.Add(StateType.Fight, EnterState_Fight);
        m_stateHandlers.Add(StateType.Comic, EnterState_Comic);

        m_currentState = new StateHandler(EnterState_Fight);
        ToggleState(StateType.Fight, true);
    }

    private void Update()
    {
        if (IsInitialized)
        {
            m_currentState();
        }
    }

    public void ChangeState(StateType targetState)
    {
        CurrentState = targetState;
    }

    public void SetPlayerHealthData(int playerId)
    {
        m_playerHealth.healthAmountSprites = m_healthData.sprites[playerId].healthSprites;
    }

    public void SetOpponentHealthData(int playerId)
    {
        m_opponentHealth.healthAmountSprites = m_healthData.sprites[playerId].healthSprites;
    }

    public void SetPlayerHealth(int amount)
    {
        m_playerHealth.SetHealthImage(amount);
    }

    public void SetOpponentHealth(int amount)
    {
        m_opponentHealth.SetHealthImage(amount);
    }

    #region States
    private void EnterState_Fight()
    {
        CurrentState = StateType.Fight;

        m_currentState = State_Fight;
    }

    private void State_Fight()
    {
        if(CurrentState != StateType.Fight)
        {
            ExitState_Fight(m_stateHandlers[CurrentState]);
        }
    }

    private void ExitState_Fight(StateHandler targetState)
    {
        ToggleState(CurrentState, false);
        m_currentState = targetState;
    }

    private void EnterState_Comic()
    {
        CurrentState = StateType.Comic;
        m_comicStripController.gameObject.SetActive(true);
        m_currentState = State_Comic;
    }

    private void State_Comic()
    {
        if (!m_comicStripController.IsPlaying) ChangeState(StateType.Fight);

        if (CurrentState != StateType.Comic)
        {
            ExitState_Comic(m_stateHandlers[CurrentState]);
        }
    }

    private void ExitState_Comic(StateHandler targetState)
    {
        ToggleState(CurrentState, false);
        m_currentState = targetState;
    }
    #endregion  // ~States

    private void ToggleState(StateType targetState, bool isEnabled)
    {
        for (int i = 0; i < m_stateObjects.Length; i++)
        {
            if (m_stateObjects[i].type == targetState)
            {
                m_stateObjects[i].ToggleState(isEnabled);
            }
        }
    }
}
