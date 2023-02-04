using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Player Models Data", menuName = "Data/Players", order = 1)]
public class PlayerModels : ScriptableObject
{
    [System.Serializable]
    public class PlayerModel
    {
        public GameObject playerObject;
        public Vector3 eulerOffset;
    }

    public PlayerModel[] models;
}
