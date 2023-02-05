using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HighConnectLayer : MonoBehaviour
{
    [SerializeField]
    private GameActions gameActions;
    [SerializeField]
    private UnityAction<int> onRecievedAttack;


    public void SetPlayer(int playerTypeId)
    {
        PlayerCharacterMode playerCharacterMode = (PlayerCharacterMode)playerTypeId;
        switch (playerCharacterMode)
        {
            case PlayerCharacterMode.CHIKA:
                {
                    gameActions.TryRegisterAsChika();
                }
                break;
            case PlayerCharacterMode.GOPSTOP:
                {
                    gameActions.TryRegisterAsGopstop();
                }
                break;
        }
    }

    public void SentRemoteAttack()
    {
        gameActions.SentMyHit();
    }

    public void OnRecieveHit(PlayerCharacterMode playerCharacterMode)
    {
        onRecievedAttack.Invoke((int)playerCharacterMode);
    }
}
