using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PlayerCharacterMode
{
    GOPSTOP = 'G',
    CHIKA = 'C',
    LOOKYLOOS = 'L',
    UNKNOWN = 'U'
}
public class PlayerCharacter
{
    public PlayerCharacterMode mode;
    public string ip;
    public byte beerCount;

    public static PlayerCharacterMode ConvertStringToPlayerCharacterMode(char input)
    {
        switch (input)
        {
            case 'G':
                {
                    return PlayerCharacterMode.GOPSTOP;
                }
            case 'C':
                {
                    return PlayerCharacterMode.CHIKA;
                }
            case 'L':
                {
                    return PlayerCharacterMode.LOOKYLOOS;
                }
            default:
                {
                    return PlayerCharacterMode.UNKNOWN;
                }
        }
    }
    public PlayerCharacter(string ip, PlayerCharacterMode mode)
    {
        beerCount = 5;
        this.mode = mode;
        this.ip = ip;
    }
}
public class GameNetworkCore : MonoBehaviour
{
    public List<PlayerCharacter> players = new List<PlayerCharacter>();
    [SerializeField]
    private NetworkSyncManager networkSyncManager;
    [SerializeField]
    private LocalNetworkLookup networkLookup;

    [SerializeField]
    private TMPro.TextMeshProUGUI usedRoles;
    [SerializeField]
    private TMPro.TextMeshProUGUI myRole;

    private PlayerCharacterMode expectedMode = PlayerCharacterMode.UNKNOWN;
    public int responseCount = 0;
    public int successCount = 0;
    public bool isRolesPoolLocked = false;

    public PlayerCharacter GetMyPlayer()
    {
        return GetPlayerByIp(networkLookup.localIP);
    }
    public PlayerCharacter GetPlayerByIp(string ip)
    {
        return players.Find(item => item.ip.Equals(ip));
    }
    public PlayerCharacter GetMyOpponent()
    {
        PlayerCharacterMode myCharacterMode = GetMyPlayer().mode;
        switch (myCharacterMode)
        {
            case PlayerCharacterMode.CHIKA:
                {
                    return players.Find(item => item.mode == PlayerCharacterMode.GOPSTOP);
                }
            case PlayerCharacterMode.GOPSTOP:
                {
                    return players.Find(item=>item.mode==PlayerCharacterMode.CHIKA);
                }
            default:
                {
                    return null;
                }
        }
    }
    //
    private void LockRoles(bool isLocked)
    {
        //needs to be implemented
        isRolesPoolLocked = isLocked;
        responseCount = 0;
        successCount = 0;
        expectedMode = PlayerCharacterMode.UNKNOWN;
    }
    //MY books
    //send
    public void TryToBook(PlayerCharacterMode mode)
    {
        if (!isRolesPoolLocked)
        {
            LockRoles(true);//LOCK ROLES
            expectedMode = mode;
            networkSyncManager.BroadcastUdpMessage("ttb " + (char)mode + networkLookup.localIP, networkLookup.clientsUN.Select(item => item.ipAddress).ToList(), networkLookup.localPort);
        }
    }
    //recieve
    public void BookResponseSucces(string messageFromIp) //for "i can/cannot be that role"
    {
        Debug.Log(messageFromIp);
        Debug.Log(networkLookup.clientsUN.Count);
        Debug.Log(expectedMode);
        responseCount++;
        successCount++;
        if(responseCount == networkLookup.clientsUN.Count)
        {
            Debug.Log("+++");
            if (responseCount == successCount)
            {
                Debug.Log("++++");
                networkSyncManager.BroadcastUdpMessage("ttr " + (char)expectedMode + networkLookup.localIP, networkLookup.clientsUN.Select(item => item.ipAddress).ToList(), networkLookup.localPort);
                players.Add(new PlayerCharacter(networkLookup.localIP, expectedMode));

                switch (expectedMode)
                {
                    case PlayerCharacterMode.GOPSTOP:
                        {
                          myRole.text += "GOPSTOP ";
                        }
                        break;
                    case PlayerCharacterMode.CHIKA:
                        {
                           myRole.text += "CHIKA ";
                        }
                        break;
                    case PlayerCharacterMode.LOOKYLOOS:
                        {
                            myRole.text += "LOOKYLOOS ";
                        }
                        break;
                }
                //SO succes EVENT + answer to all
            }
            else
            {
                networkSyncManager.BroadcastUdpMessage("ttf " + (char)expectedMode + networkLookup.localIP, networkLookup.clientsUN.Select(item => item.ipAddress).ToList(), networkLookup.localPort);

                //SO FAIL EVENT + answer to all
            }
            LockRoles(false);//SEND FAIL/MYROLE
        }
    }
    public void BookResponseFail(string messageFromIp) //for "i can/cannot be that role"
    {
        responseCount++;
        if (responseCount == networkLookup.clientsUN.Count)
        {
            networkSyncManager.BroadcastUdpMessage("ttf " + (char)expectedMode + networkLookup.localIP, networkLookup.clientsUN.Select(item => item.ipAddress).ToList(), networkLookup.localPort);

            //SO FAIL EVENT + answer to all
            LockRoles(false);//SEND FAIL/MYROLE
        }
    }
    //Others messages
    //recieve and send
    public void CheckRole(string fromMsg)
    {
        LockRoles(true);//LOCK ROLES
        //CHECK ROLE
        PlayerCharacterMode checkingRole = PlayerCharacter.ConvertStringToPlayerCharacterMode(fromMsg[0]);
        string remoteIp = fromMsg.Substring(fromMsg.LastIndexOf(checkingRole.ToString()) + 2);
        Debug.Log("+++++++++");
        Debug.Log("+"+remoteIp+"+");
        if ((checkingRole!=PlayerCharacterMode.UNKNOWN)&&(players.Count == 0||checkingRole==PlayerCharacterMode.LOOKYLOOS||players.Find(item=>item.mode==checkingRole)==null))
        {
            //Send yep
            networkSyncManager.SendUdpMessage("tty " + networkLookup.localIP, remoteIp, networkLookup.localPort);

            Debug.Log("y");
        }
        else
        {
            //send nope
            networkSyncManager.SendUdpMessage("ttn" + networkLookup.localIP, remoteIp, networkLookup.localPort);

            Debug.Log("n");
        }
    }
    public void BookOthersRoleSuccess(string fromMsg)
    {
        PlayerCharacterMode remoteRole = PlayerCharacter.ConvertStringToPlayerCharacterMode(fromMsg[0]);
        string remoteIp = fromMsg.Substring(fromMsg.LastIndexOf(remoteRole.ToString()) + 2);
        players.Add(new PlayerCharacter(remoteIp, remoteRole));
        LockRoles(false);//unlock ROLES go from lobby to game
        //GLOBAL EVENTO
        switch (remoteRole)
        {
            case PlayerCharacterMode.GOPSTOP:
                {
                    usedRoles.text += "GOPSTOP ";
                }
                break;
            case PlayerCharacterMode.CHIKA:
                {
                    usedRoles.text += "CHIKA ";
                }
                break;
            case PlayerCharacterMode.LOOKYLOOS:
                {
                    usedRoles.text += "LOOKYLOOS ";
                }
                break;
        }
    }
    public void BookOthersRoleFail(string fromMsg)
    {
        LockRoles(false);//unlock ROLES go from lobby to game
    }
}
