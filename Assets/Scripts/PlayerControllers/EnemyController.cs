using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [HideInInspector] public Transform playerTransform;

    [SerializeField] private PlayerModels m_playerModels;

    [SerializeField] private Animator m_animator;

    private float m_defenceAnimationTimer = 0f;
    private bool m_inDefence;

    private readonly int m_attackAnimationHash = Animator.StringToHash("Attack");
    private readonly int m_blockDefenceAnimationHash = Animator.StringToHash("Block");
    private readonly int m_hitAnimationHash = Animator.StringToHash("Hit");
    private readonly int m_attackIdAnimationHash = Animator.StringToHash("AttackId");
    private readonly int m_defenceIdAnimationHash = Animator.StringToHash("DefenceId");


    private void Update()
    {
        Quaternion lookRotation = Quaternion.LookRotation(playerTransform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 20f * Time.deltaTime);

        if (m_inDefence)
        {
            if (m_defenceAnimationTimer <= 0f)
            {
                m_animator.SetBool(m_blockDefenceAnimationHash, false);
                m_inDefence = false;
            }

            m_defenceAnimationTimer -= Time.deltaTime;
        }
    }

    public void SpawnModel(int index)
    {
        m_animator = Instantiate<GameObject>(m_playerModels.models[index].playerObject, transform).GetComponentInChildren<Animator>();
        m_animator.transform.parent.eulerAngles = m_playerModels.models[index].eulerOffset;
    }

    public void SetHit()
    {
        m_animator.SetTrigger(m_hitAnimationHash);
    }

    public void ExecuteCommand(CommandSynonyms.ActionType actionType)
    {
        switch (actionType)
        {
            case CommandSynonyms.ActionType.Attack:
                m_animator.SetInteger(m_attackIdAnimationHash, Random.Range(0, 5));
                m_animator.SetTrigger(m_attackAnimationHash);
                break;
            case CommandSynonyms.ActionType.Defence:
                m_animator.SetInteger(m_defenceIdAnimationHash, Random.Range(0, 4));
                m_animator.SetBool(m_blockDefenceAnimationHash, true);
                m_defenceAnimationTimer = 1.5f;
                m_inDefence = true;
                break;
            default:
                break;
        }
    }
}
