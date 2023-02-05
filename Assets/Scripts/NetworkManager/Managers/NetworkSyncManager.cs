using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class NetworkSyncManager : MonoBehaviour
{
    public static NetworkSyncManager instance = null;

    [Header("Server paramertrs")]
    public int localPort = 3238;
    //public int remotePort = 3234;
    [Header("Events")]
    public UnityEvent<string> onUdpMessageRecived;


    private Thread recieveThread;
    private event Action mainThreadQueuedCallbacks;
    private event Action eventsClone;

    private UdpClient receiver;

    private bool isNetworkActive = true;

    void Awake()
    {
        /*
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        InitializeManager();

        */
        recieveThread = new Thread(ReciverLoop);
        recieveThread.Start();
    }
    private void ReciverLoop()
    {
        receiver = new UdpClient(localPort);
        IPEndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);
        {
            while (isNetworkActive)
            {
                byte[] data = receiver.Receive(ref remoteIp);
                string recevedText = Encoding.ASCII.GetString(data);
                mainThreadQueuedCallbacks += () => {
                    onUdpMessageRecived?.Invoke(recevedText);
                };
            }
            Debug.Log("Closing listener");
            receiver.Close();
        }
    }
    public void BroadcastUdpMessage(string message, List<string> clientsAdress, int remotePort)
    {
        for (int i = 0; i < clientsAdress.Count; i++)
        {
            SendUdpMessage(message, clientsAdress[i], remotePort);
        }
    }

    public void SendUdpMessage(string message, string remoteAddress, int remotePort)
    {
        UdpClient sender = new UdpClient();
        try
        {
            byte[] dataBytes = Encoding.ASCII.GetBytes(message);
            sender.Send(dataBytes, dataBytes.Length, remoteAddress, remotePort);
            sender.Close();
        }
        catch
        {
        }
    }
    private void Update()
    {
        if (mainThreadQueuedCallbacks != null)
        {
            eventsClone = mainThreadQueuedCallbacks;
            mainThreadQueuedCallbacks = null;
            eventsClone.Invoke();
            eventsClone = null;
        }
    }
    private void OnApplicationQuit()
    {
        isNetworkActive = false;
        recieveThread.Abort();
        try
        {
            receiver.Close();
        }
        catch
        {
            //YEAHH HERE IS NOTHING YAYYYY
        }
        Debug.Log("END");
    }
}
