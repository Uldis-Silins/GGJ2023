using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parser : MonoBehaviour
{
    public ParserElement[] elements;
    public void OnDataRecived(string serialMessage)
    {
        for (int i = 0; i < elements.Length; i++)
        {
            if (serialMessage[0] == elements[i].command[0] && serialMessage[1] == elements[i].command[1] && serialMessage[2] == elements[i].command[2])
            {
                elements[i].Action.Invoke(serialMessage.Substring(4));
            }
        }
    }
}
