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
    private GameNetworkCore gameNetworkCore;
    [SerializeField]
    private TMPro.TextMeshProUGUI gopstopStatusText;
    [SerializeField]
    private TMPro.TextMeshProUGUI chikaStatusText;
    [SerializeField]
    private HighConnectLayer highConnectLayer;
    //SENDING
    //reg role
    public void TryRegisterAsGopstop()
    {
        gameNetworkCore.TryToBook(PlayerCharacterMode.GOPSTOP);
    }
    public void TryRegisterAsChika()
    {
        gameNetworkCore.TryToBook(PlayerCharacterMode.CHIKA);
    }
    public void TryRegisterAsLookyloos()
    {
        gameNetworkCore.TryToBook(PlayerCharacterMode.LOOKYLOOS);
    }
    //SENDING
    //actions
    private void SendAction(string actionMsg, PlayerCharacter playerCHaracter)//FOR: hit, pch, blk
    {
        if (localNetworkLookup.clientsUN.Count > 0 )
        {
            networkSyncManager.SendUdpMessage(actionMsg + localNetworkLookup.localIP, playerCHaracter.ip, localNetworkLookup.localPort);
            //OLD: networkSyncManager.SendUdpMessage(actionMsg + localNetworkLookup.localIP, localNetworkLookup.clientsUN[0].ipAddress, localNetworkLookup.localPort);
        }
    }
    private void BroadcastHealthAction(string actionMsg, PlayerCharacter playerCHaracter)//FOR: hit, pch, blk
    {
        for(int i=0; i<localNetworkLookup.clientsUN.Count; i++)
        {
            networkSyncManager.SendUdpMessage(actionMsg + playerCHaracter.beerCount + playerCHaracter.ip, localNetworkLookup.clientsUN[i].ipAddress, localNetworkLookup.localPort);
        }
        networkSyncManager.SendUdpMessage(actionMsg + playerCHaracter.beerCount + playerCHaracter.ip, "127.0.0.1", localNetworkLookup.localPort);
    }
    public void SentMyHit()
    {
        SendAction("hit M", gameNetworkCore.GetMyOpponent()); 
        //myStatusText.text = "Doing HIT";
    }

    public void SentMyPunch()
    {
        SendAction("pch M", gameNetworkCore.GetMyOpponent());
        //myStatusText.text = "Doing PUNCH";
    }

    public void SentMyBlock()
    {
        SendAction("blk M", gameNetworkCore.GetMyOpponent());
        //myStatusText.text = "Doing BLOCK";
    }
    private void BroadcastOponentHit(PlayerCharacter playerCharacter)
    {
        BroadcastHealthAction("hit O", playerCharacter);
    }

    private void BroadcastOponentPunch(PlayerCharacter playerCharacter)
    {
        BroadcastHealthAction("pch O", playerCharacter);
    }

    private void BroadcastOponentBlock(PlayerCharacter playerCharacter)
    {
        BroadcastHealthAction("blk O", playerCharacter);
    }
    //RECIEVING
    //actions
    public void ReciveHit(string actionFrom)
    {
        //opponentStatusText.text = actionFrom;
        //IN CASE IF MORE THAN 2 clients, message.Substring(message.LastIndexOf('O') + 2
        if (actionFrom[0] == 'O')
        {

            //opponentStatusText.text = "Opponent got HIT!";
          //  Debug.Log();
          //  Debug.Log(actionFrom[actionFrom.LastIndexOf('O') + 1]);
          PlayerCharacter playerCharacter = gameNetworkCore.GetPlayerByIp(actionFrom.Substring(actionFrom.LastIndexOf('O') + 2));
            playerCharacter.beerCount = byte.Parse(actionFrom[actionFrom.LastIndexOf('O') + 1].ToString());
            highConnectLayer.OnRecieveHit(playerCharacter.mode);
            if (playerCharacter.mode == PlayerCharacterMode.CHIKA && chikaStatusText)
            {
                chikaStatusText.text = "HP " + playerCharacter.beerCount + "Injured by: HIT";
            }
            else if(playerCharacter.mode == PlayerCharacterMode.GOPSTOP && gopstopStatusText)
            {
                gopstopStatusText.text = "HP " + playerCharacter.beerCount + "Injured by: HIT";
            }

        }
        else if(actionFrom[0] == 'M')
        {
            //myStatusText.text = "I got HIT";
            //opponentStatusText.text = "ATTACKING";
            PlayerCharacter player = gameNetworkCore.GetMyPlayer();
          //  player.beerCount--;
            BroadcastOponentHit(player);
        }
    }

    public void RecivePunch(string actionFrom)
    {
        //IN CASE IF MORE THAN 2 clients, message.Substring(message.LastIndexOf('O') + 2
        if (actionFrom[0] == 'O')
        {
            //  opponentStatusText.text = "Opponent got PUNCH!";
            PlayerCharacter playerCharacter = gameNetworkCore.GetPlayerByIp(actionFrom.Substring(actionFrom.LastIndexOf('O') + 2));
            playerCharacter.beerCount = byte.Parse(actionFrom[actionFrom.LastIndexOf('O') + 1].ToString());
            highConnectLayer.OnRecievePunch(playerCharacter.mode);
            if (playerCharacter.mode == PlayerCharacterMode.CHIKA && chikaStatusText)
            {
                chikaStatusText.text = "HP " + playerCharacter.beerCount + "Injured by: PUNCH";
            }
            else if (playerCharacter.mode == PlayerCharacterMode.GOPSTOP && gopstopStatusText)
            {
                gopstopStatusText.text = "HP " + playerCharacter.beerCount + "Injured by: PUNCH";
            }
        }
        else if (actionFrom[0] == 'M')
        {
            //myStatusText.text = "I got PUNCH";
            //opponentStatusText.text = "ATTACKING";
            PlayerCharacter player = gameNetworkCore.GetMyPlayer();
        //    player.beerCount--;
            BroadcastOponentPunch(player);
        }
    }

    public void ReciveBlock(string actionFrom)
    {
        //IN CASE IF MORE THAN 2 clients, message.Substring(message.LastIndexOf('O') + 2
        if (actionFrom[0] == 'O')
        {
            PlayerCharacter playerCharacter = gameNetworkCore.GetPlayerByIp(actionFrom.Substring(actionFrom.LastIndexOf('O') + 2));
            playerCharacter.beerCount = byte.Parse(actionFrom[actionFrom.LastIndexOf('O') + 1].ToString());
            highConnectLayer.OnRecieveBlock(playerCharacter.mode);
            PlayerCharacter myPlayer = gameNetworkCore.GetMyPlayer();
            if (playerCharacter.mode == PlayerCharacterMode.GOPSTOP && chikaStatusText)
            {
                if(myPlayer.mode == PlayerCharacterMode.CHIKA)
                {
                    StartCoroutine(BlockDelay());
                }
                chikaStatusText.text = "HP " + playerCharacter.beerCount + "Blocking";
            }
            else if (playerCharacter.mode == PlayerCharacterMode.CHIKA && gopstopStatusText)
            {
                if (myPlayer.mode == PlayerCharacterMode.GOPSTOP)
                {
                    StartCoroutine(BlockDelay());
                }
                gopstopStatusText.text = "HP " + playerCharacter.beerCount + "Blocking";
            }
        }
        else if (actionFrom[0] == 'M')
        {
            // myStatusText.text = "I got BLOCK";
            // opponentStatusText.text = "Saving";
            PlayerCharacter player = gameNetworkCore.GetMyPlayer();
            BroadcastOponentBlock(player);
        }
    }
    public void ReciveBlockOff(string actionFrom)
    {
            PlayerCharacter playerCharacter = gameNetworkCore.GetPlayerByIp(actionFrom.Substring(actionFrom.LastIndexOf('O') + 2));
            playerCharacter.beerCount = byte.Parse(actionFrom[actionFrom.LastIndexOf('O') + 1].ToString());
            highConnectLayer.OnRecieveBlockOff(playerCharacter.mode);
            if (playerCharacter.mode == PlayerCharacterMode.CHIKA && chikaStatusText)
            {
                chikaStatusText.text = "HP " + playerCharacter.beerCount + "BlockOff";
            }
            else if (playerCharacter.mode == PlayerCharacterMode.GOPSTOP && gopstopStatusText)
            {
                gopstopStatusText.text = "HP " + playerCharacter.beerCount + "BlockOff";
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

    IEnumerator BlockDelay()
    {
        yield return new WaitForSeconds(1.5f);
        PlayerCharacter player = gameNetworkCore.GetMyPlayer();
        BroadcastHealthAction("blj O", player);
    }
}
