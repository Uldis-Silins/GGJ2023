using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HighConnectLayer : MonoBehaviour
{
    [SerializeField]
    private GameActions gameActions;
    public UnityAction<int> onRecievedAttack;
    public UnityAction<int> onRecievedBlock;
    public UnityAction<int> onRecievedBlockOff;
    public UnityAction<int> onRecievedPunch;

    public UnityAction onNetworkReady;
    public UnityAction onGameReady;



    public void SetPlayer(int playerTypeId)
    {
        if(playerTypeId == 0)
        {
            gameActions.TryRegisterAsGopstop();
        }
        else if(playerTypeId == 1)
        {
            gameActions.TryRegisterAsChika();
        }
        else
        {
            gameActions.TryRegisterAsLookyloos();
        }
    }
    
    public void SentRemoteAttack()
    {
        gameActions.SentMyHit();
    }
    public void SentRemotePunch()
    {
        gameActions.SentMyPunch();
    }
    public void SentRemoteBlock()
    {
        gameActions.SentMyBlock();
    }

    public void OnRecieveHit(PlayerCharacterMode playerCharacterMode)
    {
        if (playerCharacterMode == PlayerCharacterMode.GOPSTOP)
        {
            onRecievedAttack?.Invoke(0);
        }
        else if (playerCharacterMode == PlayerCharacterMode.CHIKA)
        {
            onRecievedAttack?.Invoke(1);
        }
    }
    public void OnRecievePunch(PlayerCharacterMode playerCharacterMode)
    {
        if (playerCharacterMode == PlayerCharacterMode.GOPSTOP)
        {
            onRecievedPunch?.Invoke(0);
        }
        else if (playerCharacterMode == PlayerCharacterMode.CHIKA)
        {
            onRecievedPunch?.Invoke(1);
        }
    }
    public void OnRecieveBlock(PlayerCharacterMode playerCharacterMode)
    {
        if (playerCharacterMode == PlayerCharacterMode.GOPSTOP)
        {
            onRecievedBlock?.Invoke(1);
        }
        else if (playerCharacterMode == PlayerCharacterMode.CHIKA)
        {
            onRecievedBlock?.Invoke(0);
        }
    }
    public void OnRecieveBlockOff(PlayerCharacterMode playerCharacterMode)
    {
        if (playerCharacterMode == PlayerCharacterMode.GOPSTOP)
        {
            onRecievedBlockOff?.Invoke(0);
        }
        else if (playerCharacterMode == PlayerCharacterMode.CHIKA)
        {
            onRecievedBlockOff?.Invoke(1);
        }
    }

    public void OnRecieveNetworkReady()//ready do not touch
    {
        onNetworkReady?.Invoke();
    }
    public void OnGameReady()
    {
        onGameReady?.Invoke();
    }
}
