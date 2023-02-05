using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HighConnectLayer : MonoBehaviour
{
    [SerializeField]
    private GameActions gameActions;
    public UnityAction<int> onRecievedAttack;


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

    public void OnRecieveHit(PlayerCharacterMode playerCharacterMode)
    {
        onRecievedAttack?.Invoke((int)playerCharacterMode);
    }
}
