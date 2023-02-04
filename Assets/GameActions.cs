using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActions : MonoBehaviour
{
    [SerializeField]
    private LocalNetworkLookup localNetworkLookup;
    [SerializeField]
    private NetworkSyncManager networkSyncManager;
    [SerializeField]
    private TMPro.TextMeshProUGUI myStatusText;
    [SerializeField]
    private TMPro.TextMeshProUGUI opponentStatusText;
    //SENDING
    //reg role
    void RegisterAsGopstop()
    {

    }
    void RegisterAsChika()
    {

    }
    void RegisterAsLookyloos()
    {

    }
    //SENDING
    //actions
    private void SendAction(string actionMsg)//hit, pch, blk
    {
        if (localNetworkLookup.clientsUN.Count > 0 && localNetworkLookup.clientsUN[0].isOnline)
        {
            networkSyncManager.SendUdpMessage(actionMsg + localNetworkLookup.localIP, localNetworkLookup.clientsUN[0].ipAddress, localNetworkLookup.localPort);
        }
    }
    public void SentMyHit()
    {
        SendAction("hit M");
        myStatusText.text = "Doing HIT";
    }

    public void SentMyPunch()
    {
        SendAction("pch M");
        myStatusText.text = "Doing PUNCH";
    }

    public void SentMyBlock()
    {
        SendAction("blk M");
        myStatusText.text = "Doing BLOCK";
    }
    private void SentOponentHit()
    {
        SendAction("hit O");
    }

    private void SentOponentPunch()
    {
        SendAction("pch O");
    }

    private void SentOponentBlock()
    {
        SendAction("blk O");
    }
    //RECIEVING
    //actions
    public void ReciveHit(string actionFrom)
    {
        opponentStatusText.text = actionFrom;
        //IN CASE IF MORE THAN 2 clients, message.Substring(message.LastIndexOf('O') + 1
        if (actionFrom[0] == 'O')
        {
            opponentStatusText.text = "Opponent got HIT!";
        }
        else if(actionFrom[0] == 'M')
        {
            myStatusText.text = "I got HIT";
            opponentStatusText.text = "ATTACKING";
            SentOponentHit();
        }
    }

    public void RecivePunch(string actionFrom)
    {
        //IN CASE IF MORE THAN 2 clients, message.Substring(message.LastIndexOf('O') + 1
        if (actionFrom[0] == 'O')
        {
            opponentStatusText.text = "Opponent got PUNCH!";
        }
        else if (actionFrom[0] == 'M')
        {
            myStatusText.text = "I got PUNCH";
            opponentStatusText.text = "ATTACKING";
            SentOponentPunch();
        }
    }

    public void ReciveBlock(string actionFrom)
    {
        //IN CASE IF MORE THAN 2 clients, message.Substring(message.LastIndexOf('O') + 1
        if (actionFrom[0] == 'O')
        {
            opponentStatusText.text = "Opponent know my BLOCK!";
        }
        else if (actionFrom[0] == 'M')
        {
            myStatusText.text = "I got BLOCK";
            opponentStatusText.text = "Saving";
            SentOponentBlock();
        }
    }
    //fails
    public void ReciveAgressionFail(string actionFrom)//BecauseOfBlock
    {

    }
    public void ReciveBlockFail(string actionFrom)//Because of agression
    {

    }
    //other states
    void ReciveIdle(string actionFrom)
    {

    }
}
