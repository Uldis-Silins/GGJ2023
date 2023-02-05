using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GameController : MonoBehaviour
{
    [SerializeField] private ARPlaneManager m_arPlaneManager;
    [SerializeField] private PlayerController m_playerPrefab;
    [SerializeField] private EnemyController m_enemyPrefab;
    [SerializeField] private InputController m_inputController;
    [SerializeField] private UI_Controller m_uiController;
    [SerializeField] private HighConnectLayer m_connectLayer;

    private PlayerController m_spawnedPlayer;
    private EnemyController m_spawnedEnemy;

    private int m_myPlayerId;

    private void OnEnable()
    {
        m_arPlaneManager.planesChanged += HandlePlanesChanged;
        m_connectLayer.onRecievedAttack += ReceiveRemoteAttack;
    }

    private void OnDisable()
    {
        m_arPlaneManager.planesChanged -= HandlePlanesChanged;
        m_connectLayer.onRecievedAttack -= ReceiveRemoteAttack;
    }

    private void Start()
    {
        m_inputController.onResultReceived.AddListener(HandleCommandInput);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            m_spawnedPlayer = Instantiate<PlayerController>(m_playerPrefab, Vector3.zero, Quaternion.identity);
            m_spawnedPlayer.SpawnModel(0);
            m_spawnedPlayer.ConnectLayer = m_connectLayer;
            m_spawnedEnemy = Instantiate<EnemyController>(m_enemyPrefab, m_spawnedPlayer.EnemyPosition, Quaternion.identity);
            m_spawnedEnemy.playerTransform = m_spawnedPlayer.transform;
            m_spawnedEnemy.SpawnModel(1);
        }

        if(m_spawnedPlayer != null)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                m_spawnedPlayer.ExecuteCommand("brunch");
                if(Random.value > 0.5f) m_spawnedEnemy.ExecuteCommand(CommandSynonyms.ActionType.Defence); else m_spawnedEnemy.SetHit(Random.Range(0, 4));
            }
            else if(Input.GetKeyDown(KeyCode.S))
            {
                m_spawnedPlayer.ExecuteCommand("black");
                m_spawnedEnemy.ExecuteCommand(CommandSynonyms.ActionType.Attack);
            }
            else if(Input.GetKeyDown(KeyCode.D))
            {
                m_spawnedPlayer.SetHit(Random.Range(0, 4));
                m_spawnedEnemy.ExecuteCommand(CommandSynonyms.ActionType.Attack);
            }
        }
    }
#endif

    public void OnConnectClick(int playerId)
    {
        m_myPlayerId = playerId;
        m_connectLayer.SetPlayer(playerId);
    }

    private void HandlePlanesChanged(ARPlanesChangedEventArgs args)
    {
        bool removeListener = false;

        foreach (var plane in args.updated)
        {
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp && plane.extents.magnitude > 2f && m_spawnedPlayer == null)
            {
                Vector3 position = plane.center;
                m_spawnedPlayer = Instantiate<PlayerController>(m_playerPrefab, position, Quaternion.identity);
                m_spawnedPlayer.SpawnModel(0);
                m_spawnedPlayer.ConnectLayer = m_connectLayer;
                m_spawnedEnemy = Instantiate<EnemyController>(m_enemyPrefab, m_spawnedPlayer.EnemyPosition, Quaternion.identity);
                m_spawnedEnemy.playerTransform = m_spawnedPlayer.transform;
                m_spawnedEnemy.SpawnModel(1);
                removeListener = true;
            }
        }

        if(removeListener)
        {
            m_arPlaneManager.planesChanged -= HandlePlanesChanged;
        }
    }

    private void HandleCommandInput(string command)
    {
        if(m_spawnedPlayer != null)
        {
            m_spawnedPlayer.ExecuteCommand(command);
        }
    }

    public void ReceiveRemoteAttack(int playerId)
    {
        if(playerId != m_myPlayerId)
        {
            m_spawnedEnemy.ExecuteCommand(CommandSynonyms.ActionType.Attack);
        }
    }
}
