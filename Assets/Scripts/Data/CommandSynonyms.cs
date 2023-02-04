using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Synonyms Data", menuName = "Data/Commands", order = 1)]
public class CommandSynonyms : ScriptableObject
{
    public enum ActionType { None, Attack, Defence }

    [SerializeField] private List<string> m_hitSynonyms;
    [SerializeField] private List<string> m_punchSynonyms;
    [SerializeField] private List<string> m_blockSynonyms;

    public ActionType GetActionFromText(string commandText)
    {
        ActionType type = ActionType.None;

        if (m_hitSynonyms.Contains(commandText)) type = ActionType.Attack;
        else if (m_punchSynonyms.Contains(commandText)) type = ActionType.Attack;
        else if (m_blockSynonyms.Contains(commandText)) type = ActionType.Defence;

        return type;
    }
}
