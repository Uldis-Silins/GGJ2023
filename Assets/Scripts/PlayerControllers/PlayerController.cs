using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private CommandSynonyms m_commandData;

	[SerializeField] private GameObject[] m_playerModels;
	[SerializeField] private Transform m_enemyPositionTransform;

	[SerializeField] private Animator m_animator;

	private float m_defenceAnimationTimer = 0f;
	private bool m_inDefence;

	private readonly int m_hookAttackAnimationHash = Animator.StringToHash("HookAttack");
	private readonly int m_uppercutAttackAnimationHash = Animator.StringToHash("UppercutAttack");
	private readonly int m_blockDefenceAnimationHash = Animator.StringToHash("Block");
	private readonly int m_hitAnimationHash = Animator.StringToHash("Hit");

	public Vector3 EnemyPosition { get { return m_enemyPositionTransform.position; } }

    private void Update()
    {
        if(m_inDefence)
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
		m_animator = Instantiate<GameObject>(m_playerModels[index], transform).GetComponentInChildren<Animator>();
	}

	public void SetHit()
	{
		m_animator.SetTrigger(m_hitAnimationHash);
	}

    public void ExecuteCommand(string command)
	{
		ExecuteCommand(m_commandData.GetActionFromText(command));
	}

	private void ExecuteCommand(CommandSynonyms.ActionType actionType)
	{
		switch (actionType)
		{
			case CommandSynonyms.ActionType.Attack:
				m_animator.SetTrigger(Random.value > 0.5f ? m_hookAttackAnimationHash : m_uppercutAttackAnimationHash);
				break;
			case CommandSynonyms.ActionType.Defence:
				m_animator.SetBool(m_blockDefenceAnimationHash, true);
				m_defenceAnimationTimer = 1.5f;
				m_inDefence = true;
				break;
			default:
				break;
		}
	}
}

