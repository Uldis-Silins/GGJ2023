using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum NetworkGameMode
{
    IDLE,
    GAMEINPROGRESS,
    UNKNOWN
}

public enum PlayerType
{
    FIGHTER,
    OBSERVER
}


//clear mess
/*public enum NetworkLoadingState//to implement
{
    INIT,
    LOADING,
    WAITING
}*/

public class SyncManager : MonoBehaviour
{
    private class RemotePlayerStete
    {
        public NetworkGameMode networkGameMode;
        public PlayerType networkType;
        public string ipAdress;
        //public int portAdress;
        public bool isOnline;
        public bool isBSPlaying;
    }
    public static SyncManager instance = null;
    public LocalNetworkLookup localNetworkLookup;
    public int targetClients;//to implement
    private string localIpAdress;
    private int localPortAdress;
    private bool isBSPlaying = false;

    public UnityEvent onStopedAll;
    public UnityEvent onStartedOne;


    private NetworkGameMode _localNetworkGameMode = NetworkGameMode.IDLE;
    public NetworkGameMode localNetworkGameMode
    {
        get
        {
            return _localNetworkGameMode;
        }
        set
        {
            _localNetworkGameMode = value;
            BroadcastGameState();
        }
    }

    public TextMeshProUGUI textDBG;

    private List<RemotePlayerStete> remotePlayerStetes;

    void Start()
    {
        remotePlayerStetes = new List<RemotePlayerStete>();
        localIpAdress = localNetworkLookup.localIP;
        localPortAdress = localNetworkLookup.localPort;
        localNetworkGameMode = NetworkGameMode.IDLE;
        //NetworkManager.instance.GetComponent<Parser>().OnDataRecived("set IDLE12");
    }
    public void OnNetworkSetupIsReady()
    {
        CheckState();
    }

    /// <summary>
    /// ////////////////////////////
    /// </summary>
   /* public void SentTest()
    {

        NetworkManager.instance.SendUdpMessage("set IDLE3", "192.168.1.12", 3238);
    }*/
    /*public void SetRemotePlayerState(string message)
    {
        NetworkManager.instance.SendUdpMessage("set IDLE3", "192.168.1.12", 3238);
    }*/
    //message income set IDLE192.168.1.5 - tell the state
    //message income set GAMEINPROGRESS192.168.1.5 - tell the state
    //------------
    //message outcome get WHATHAPPEND127.0.0.1 - ask for state
    private void BroadcastGameState()
    {
        //Debug.LogError(remotePlayerStetes.Count);
        for (int i = 0; i < remotePlayerStetes.Count; i++)
        {
            if (remotePlayerStetes[i].isOnline)
            {
                SentGameState(remotePlayerStetes[i].ipAdress);
            }
        }
    }
    private void SentGameState(string sentToIp)
    {
        NetworkSyncManager.instance.SendUdpMessage("set " + localNetworkGameMode.ToString() + localIpAdress, sentToIp, localPortAdress);
    }
    public void GetRemotePlayerState(string message)
    {
        if (message.Contains("WHATHAPPEND"))
        {
            string playerIpAdress = message.Substring(message.LastIndexOf('D') + 1);
            SentGameState(playerIpAdress);
        }
    }
    private void AddRemotePlayer(string playerIpAdress)
    {
        RemotePlayerStete remotePlayer = new RemotePlayerStete();
        remotePlayer.ipAdress = playerIpAdress;
        remotePlayer.networkGameMode = NetworkGameMode.IDLE;
        remotePlayer.isOnline = true;
        remotePlayer.isBSPlaying = false;
        remotePlayerStetes.Add(remotePlayer);
    }
    private void DeliteRemotePlayer(int index)
    {

    }
    private void BroadcastPlayBS(bool isPlayVideo)
    {
        string bsCommand = isPlayVideo ? "ON" : "OFF";
        //SIMPLE VERSION
        localNetworkLookup.BroadcastMessageToEveryone(bsCommand, localNetworkLookup.localPort);

        //FEEDBACK VERSION
        /*for(int i=0; i<remotePlayerStetes.Count; i++)
        {
            NetworkManager.instance.SendUdpMessage("set BS_STAT" + bsCommand, remotePlayerStetes[i].ipAdress, localPortAdress);
        }
        for (int i=0; i< localNetworkLookup.clientsBR.Count; i++)
        {
            if (localNetworkLookup.clientsBR[i].isOnline)
            {
                NetworkManager.instance.SendUdpMessage(bsCommand, remotePlayerStetes[i].ipAdress, localPortAdress);
            }
        }*/

    }
    private void CheckState()
    {
        bool isPlaying = false;
        bool newIsBsPlaying = false;
        int bsInfoTrueStates = 0;
        for (int i = 0; i < remotePlayerStetes.Count; i++)
        {
            if (remotePlayerStetes[i].networkGameMode == NetworkGameMode.GAMEINPROGRESS)
            {
                isPlaying = true;
            }
            if (remotePlayerStetes[i].isBSPlaying)
            {
                newIsBsPlaying = true;
                bsInfoTrueStates++;
            }
        }
        if (localNetworkGameMode == NetworkGameMode.GAMEINPROGRESS)
        {
            isPlaying = true;
        }
        if (isPlaying)
        {
            Debug.LogError("Someone is playing");
            //textDBG.text = (localNetworkGameMode == NetworkGameMode.GAMEINPROGRESS)? "SOMEONE IS PLAYING AND MEE TO" : "SOMEONE IS PLAYING";
            if (isBSPlaying)
            {
                //SENT TO ALL THAT BS IS STOPPED PLAYBACK
                //sent to BS TO STOP PLAY
                isBSPlaying = false;
                Debug.LogError("I stoped play video");
                BroadcastPlayBS(isBSPlaying);
                onStartedOne.Invoke();
            }
        }
        else
        {
            //textDBG.text = "NO ONE IS PLAYING";
            if (!newIsBsPlaying)
            {
                //SENT TO ALL THAT BS IS PLAYING
                //sent to BS TO PLAY VIDEO
                isBSPlaying = true;
                //Debug.LogError("I start play video");
                BroadcastPlayBS(isBSPlaying);
                onStopedAll.Invoke();
            }
        }
    }
    private void ProceedPlayer(string message, char lastIndexChar)
    {
        string playerIpAdress = message.Substring(message.LastIndexOf(lastIndexChar) + 1);
        RemotePlayerStete remotePlayer = remotePlayerStetes.Find(item => item.ipAdress.Equals(playerIpAdress));
        if (remotePlayer == null)
        {
            AddRemotePlayer(playerIpAdress);
        }
        else
        {
            remotePlayer.networkGameMode = lastIndexChar == 'E' ? NetworkGameMode.IDLE : NetworkGameMode.GAMEINPROGRESS;
            int index = remotePlayerStetes.FindLastIndex(item => item.ipAdress.Equals(playerIpAdress));
            remotePlayer.isOnline = localNetworkLookup.clientsUN.Find(item => item.ipAddress.Equals(playerIpAdress)).isOnline;
            remotePlayerStetes[index] = remotePlayer;
        }
        CheckState();
    }
    public void PlayerWentOffline(string address)
    {
        int index = remotePlayerStetes.FindLastIndex(item => item.ipAdress.Equals(address));
        if (index != -1)
        {
            remotePlayerStetes[index].isOnline = false;
            remotePlayerStetes[index].networkGameMode = NetworkGameMode.IDLE;
        }
    }
    public void PlayerWentOnline(string address)
    {
        int index = remotePlayerStetes.FindLastIndex(item => item.ipAdress.Equals(address));
        if (index != -1)
        {
            remotePlayerStetes[index].isOnline = true;
        }
        else
        {
            AddRemotePlayer(address);
        }
    }
    public void SetRemotePlayerState(string message)
    {
        if (message.Contains("IDLE"))
        {
            ProceedPlayer(message, 'E');
        }
        else if (message.Contains("GAMEINPROGRESS"))
        {
            ProceedPlayer(message, 'S');
        }
        else if (message.Contains("BS_STAT"))
        {
            string bsStatus = message.Substring(message.LastIndexOf('T') + 1);
            int newBsState = 2;//BRAINFUCK
            if (bsStatus.Equals("ON"))
            {
                newBsState = 1;
            }
            else if (bsStatus.Equals("OFF"))
            {
                newBsState = 0;
            }
            if (newBsState < 2)
            {
                for (int i = 0; i < remotePlayerStetes.Count; i++)
                {
                    remotePlayerStetes[i].isBSPlaying = newBsState == 0 ? false : true;
                }
                Debug.LogError("Someone change BS status to - " + newBsState);
            }
        }
    }
    public void GameOn()
    {
        //ChangeGameMode(GameMode.MAPGAME);
    }
    public void GameOff()
    {
        // ChangeGameMode(GameMode.IDLE);
    }
    public void ChangeGameMode(/*GameMode newGameMode*/)
    {
        /* if (newGameMode == GameMode.IDLE)
         {
             localNetworkGameMode = NetworkGameMode.IDLE;
             CheckState();
         }
         else
         {
             localNetworkGameMode = NetworkGameMode.GAMEINPROGRESS;
             CheckState();
         }*/
    }
}
