using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GameController : MonoBehaviour
{
    public Button gopButton, chikButton;

    [SerializeField] private ARPlaneManager m_arPlaneManager;
    [SerializeField] private PlayerController m_playerPrefab;
    [SerializeField] private EnemyController m_enemyPrefab;
    [SerializeField] private InputController m_inputController;
    [SerializeField] private UI_Controller m_uiController;
    [SerializeField] private HighConnectLayer m_connectLayer;

    private PlayerController m_spawnedPlayer;
    private EnemyController m_spawnedEnemy;

    private int m_myPlayerId;

    private Vector3 m_spawnPosition;
    private bool m_canSpawn;
    private bool m_netReady;
    private bool m_inGame;

    private void OnEnable()
    {
        m_arPlaneManager.planesChanged += HandlePlanesChanged;
        m_connectLayer.onRecievedAttack += ReceiveRemoteAttack;
        m_connectLayer.onRecievedBlock += ReceiveRemoteBlock;
        m_connectLayer.onNetworkReady += ReceiveNetworkReady;
        m_connectLayer.onGameReady += ReceiveGameStart;
    }

    private void OnDisable()
    {
        m_arPlaneManager.planesChanged -= HandlePlanesChanged;
        m_connectLayer.onRecievedAttack -= ReceiveRemoteAttack;
        m_connectLayer.onRecievedBlock -= ReceiveRemoteBlock;
        m_connectLayer.onNetworkReady -= ReceiveNetworkReady;
        m_connectLayer.onGameReady -= ReceiveGameStart;
    }

    private void Start()
    {
        m_inputController.onResultReceived.AddListener(HandleCommandInput);
        gopButton.gameObject.SetActive(false);
        chikButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(m_canSpawn && m_netReady)
        {
            gopButton.gameObject.SetActive(true);
            chikButton.gameObject.SetActive(true);
            m_netReady = false;
        }

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

    public void OnConnectClick(int playerId)
    {
        if (m_canSpawn)
        {
            m_myPlayerId = playerId;
            int modelId = playerId == 0 ? 1 : 0; // 0 - gopstop, but model is id 1 and vice versa
            m_connectLayer.SetPlayer(playerId);
            m_spawnedPlayer = Instantiate<PlayerController>(m_playerPrefab, m_spawnPosition, Quaternion.identity);
            m_spawnedPlayer.SpawnModel(modelId);
            m_spawnedPlayer.ConnectLayer = m_connectLayer;
            m_spawnedEnemy = Instantiate<EnemyController>(m_enemyPrefab, m_spawnedPlayer.EnemyPosition, Quaternion.identity);
            m_spawnedEnemy.playerTransform = m_spawnedPlayer.transform;
            m_spawnedEnemy.SpawnModel(playerId);    // 0 - gopstop, but model is id 1 and vice versa

            gopButton.gameObject.SetActive(false);
            chikButton.gameObject.SetActive(false);
            m_canSpawn = false;
        }
    }

    private void HandlePlanesChanged(ARPlanesChangedEventArgs args)
    {
        bool removeListener = false;

        foreach (var plane in args.updated)
        {
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp && plane.extents.magnitude > 2f && m_spawnedPlayer == null)
            {
                m_spawnPosition = plane.center;
                m_canSpawn = true;
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
        if(m_spawnedPlayer != null && m_inGame)
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

    public void ReceiveRemoteBlock(int playerId)
    {
        if (playerId != m_myPlayerId)
        {
            m_spawnedEnemy.ExecuteCommand(CommandSynonyms.ActionType.Defence);
        }
    }

    private void ReceiveNetworkReady()
    {
        m_netReady = true;
    }

    private void ReceiveGameStart()
    {
        m_inGame = true;
    }
}
