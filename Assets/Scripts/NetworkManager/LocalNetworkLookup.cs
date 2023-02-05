using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.Events;

public class PlayerClient
{
    private string _ipAddress;
    private bool _isOnline = false;
    private float _timeoutSec = 8.0f;
    CancellationTokenSource tokenSource;
    CancellationToken cancellationToken;
    public event Action<string> OnPlayerConnected;
    public event Action<string> OnPlayerDisonnected;
    public int id = -1;
    public string ipAddress
    {
        get
        {
            return _ipAddress;
        }
    }
    public bool isOnline
    {
        get
        {
            return _isOnline;
        }
    }
    void StartTask()
    {
        tokenSource = new CancellationTokenSource();
        cancellationToken = tokenSource.Token;
        Task.Run(async () =>
        {
            await Task.Delay((int)(_timeoutSec * 1000), cancellationToken);
            _isOnline = false;
            OnPlayerDisonnected?.Invoke(ipAddress);
        }, cancellationToken);
    }
    private void Setup(string argIpAddress)
    {
        _ipAddress = string.Copy(argIpAddress);
        ToggleStillAvaiable();
    }
    public PlayerClient(float argTimeoutSec, string argIpAddress)
    {
        _timeoutSec = argTimeoutSec;
        Setup(argIpAddress);
    }
    public PlayerClient(string argIpAddress)
    {
        Setup(argIpAddress);
    }

    public void ToggleStillAvaiable()
    {
        tokenSource?.Cancel();
        StartTask();
        _isOnline = true;
        OnPlayerConnected?.Invoke(ipAddress);
    }
    public void Dispose()
    {
        OnPlayerConnected = null;
        OnPlayerDisonnected = null;
        tokenSource.Cancel();
    }
}
/// <summary>
/// /////////////////////////////////
/// </summary>
public class LocalNetworkLookup : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private TMP_Text outputTextUn;
    [SerializeField]
    private NetworkSyncManager networkSyncManager;
    public List<PlayerClient> clientsBR;
    public List<PlayerClient> clientsUN;
    public UnityEvent<string> onWentOffline;
    public UnityEvent<string> onWentOnline;
    public int myId = 0;
    public string localIP;
    public int localPort = 3238;
    public int expectedUnityCount = 3;
    public int expectedBsCount = 0;
    public UnityEvent onReady;
    private IEnumerator networkUpdater;
    private bool isStarted = false;


    IEnumerator UpdateNetwork()
    {
        while (true)
        {
            GetAllClientsBySubnet(localIP, "req WHO" + localIP, localPort);
            yield return new WaitForSeconds(5);
            //outputText.
            int i = 0;
            StringBuilder stringBuilder = new StringBuilder();
            bool isNothing = true;
            //Debug.LogError(clientsUN.Count);
            while (i < clientsUN.Count)
            {
                stringBuilder.AppendLine($"IP Address {i}: {clientsUN[i].ipAddress} online? - {clientsUN[i].isOnline}");
                System.Math.Max(System.Threading.Interlocked.Increment(ref i), i - 1);
                isNothing = false;
            }
            if (isNothing && outputTextUn)
            {
                outputTextUn.text = "Searching...";
            }
            else if (outputTextUn)
            {
                outputTextUn.text = stringBuilder.ToString();
            }


            //public int expectedUnityCount = 3;
            // public int expectedBsCount = 0;
            if (clientsBR.Count == expectedBsCount && clientsUN.Count == expectedUnityCount && !isStarted)
            {
                isStarted = true;
                onReady.Invoke();
            }
        }
    }
    private void Start()
    {
        clientsBR = new List<PlayerClient>();
        clientsUN = new List<PlayerClient>();
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                //Debug.Log(localIP);
                break;
            }
        }
        networkUpdater = UpdateNetwork();
        StartCoroutine(networkUpdater);
    }
    private void UnityEventWrapperOnConnecting(string message)
    {
        onWentOnline.Invoke(message);
    }
    private void UnityEventWrapperOnDisconnecting(string message)
    {
        onWentOffline.Invoke(message);
    }
    private void RegisterClientByIp(string clientIP, List<PlayerClient> clientList, bool isUnity)
    {
        int playerIndex = clientList.FindIndex(item => item.ipAddress.Equals(clientIP));
        if (playerIndex == -1)
        {
            PlayerClient playerClient = new PlayerClient(clientIP);
            if (isUnity)
            {
                playerClient.OnPlayerDisonnected += UnityEventWrapperOnDisconnecting;
                playerClient.OnPlayerConnected += UnityEventWrapperOnConnecting;
            }
            clientList.Add(playerClient);
        }
        else
        {
            clientList[playerIndex].ToggleStillAvaiable();
        }
    }

    public void RegisterClient(string message)
    {
        string clientIP;
        if (message.Contains("UNITY"))
        {
            clientIP = message.Substring(message.LastIndexOf('Y') + 1);
            RegisterClientByIp(clientIP, clientsUN, true);
            //onNewClientUN.Invoke(clientsUN);
        }
        else if (message.Contains("BRIGHTSIGN"))
        {
            clientIP = message.Substring(message.LastIndexOf('N') + 1);
            RegisterClientByIp(clientIP, clientsBR, true);
            //onNewClientBR.Invoke(clientsBR);
        }
    }
    public void AnswerClient(string message)
    {
        if (message.Contains("WHO"))
        {
            networkSyncManager.SendUdpMessage("res UNITY" + localIP, message.Substring(message.LastIndexOf('O') + 1), localPort);
        }
    }
    public void BroadcastMessageToEveryone(string message, int port)
    {
        GetAllClientsBySubnet(localIP, message, port);
    }
    private void GetAllClientsBySubnet(string subNetStr, string message, int port)
    {
        string subNetStrNormalized = subNetStr[..(subNetStr.LastIndexOf('.') + 1)];
        for (int i = 1; i < 255; i++)
        {
            string finalClientIpToCheck = subNetStrNormalized + i;
            if (!finalClientIpToCheck.Equals(localIP))
            {
                networkSyncManager.SendUdpMessage(message, finalClientIpToCheck, port);
            }
        }
    }
    private void ClearPlayerClient(PlayerClient playerClient)
    {
        playerClient.Dispose();
    }
    private void OnApplicationQuit()
    {
        StopCoroutine(networkUpdater);
        clientsBR.ForEach(ClearPlayerClient);
        clientsUN.ForEach(ClearPlayerClient);
    }
}
