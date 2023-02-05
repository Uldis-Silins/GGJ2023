using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private CommandSynonyms m_commandData;

	[SerializeField] private PlayerModels m_playerModels;
	[SerializeField] private Transform m_enemyPositionTransform;

	//[SerializeField] private HighConnectLayer m_connectLayer;

	private Animator m_animator;
	private HitParticlesController m_particles;

	private float m_defenceAnimationTimer = 0f;
	private bool m_inDefence;

	private readonly int m_attackAnimationHash = Animator.StringToHash("Attack");
	private readonly int m_blockDefenceAnimationHash = Animator.StringToHash("Block");
	private readonly int m_hitAnimationHash = Animator.StringToHash("Hit");
	private readonly int m_attackIdAnimationHash = Animator.StringToHash("AttackId");
	private readonly int m_defenceIdAnimationHash = Animator.StringToHash("DefenceId");
	private readonly int m_hitIdAnimationHash = Animator.StringToHash("HitId");
    private readonly int m_gameOverAnimationHash = Animator.StringToHash("Wasted");
    private readonly int m_winAnimationHash = Animator.StringToHash("Win");

    public Vector3 EnemyPosition { get { return m_enemyPositionTransform.position; } }
	//public HighConnectLayer ConnectLayer { set { if (m_connectLayer == null) m_connectLayer = value; } }

    private void Update()
    {
        Quaternion lookRotation = Quaternion.LookRotation(m_enemyPositionTransform.position - transform.position);
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
		var instance = Instantiate<GameObject>(m_playerModels.models[index].playerObject, transform);
		m_animator = instance.GetComponentInChildren<Animator>();
        m_particles = instance.GetComponent<HitParticlesController>();
		m_animator.transform.parent.eulerAngles = m_playerModels.models[index].eulerOffset;
	}

	public void SetHit(int hitId)
	{
		m_particles.PlayBloodHit(Random.Range(0,2));
		m_animator.SetInteger(m_hitIdAnimationHash, hitId);
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
				m_animator.SetInteger(m_attackIdAnimationHash, Random.Range(0, 4));
				m_animator.SetTrigger(m_attackAnimationHash);
				//m_connectLayer.SentRemoteAttack();
				break;
			case CommandSynonyms.ActionType.Defence:
				m_animator.SetInteger(m_defenceIdAnimationHash, Random.Range(0, 3));
				m_animator.SetBool(m_blockDefenceAnimationHash, true);
				m_defenceAnimationTimer = 1.5f;
				m_inDefence = true;
				//m_connectLayer.SentRemoteBlock();
				break;
			default:
				break;
		}
	}

	public void Attack(int attackID)
	{
        m_animator.SetInteger(m_attackIdAnimationHash, attackID);
        m_animator.SetTrigger(m_attackAnimationHash);
    }

	public void Defence(int defenceID)
	{
        m_animator.SetInteger(m_defenceIdAnimationHash, defenceID);
        m_animator.SetBool(m_blockDefenceAnimationHash, true);
        m_defenceAnimationTimer = 1.5f;
        m_inDefence = true;
    }

	public void GameOver()
	{
		m_animator.SetTrigger(m_gameOverAnimationHash);
	}

	public void Win()
	{
		m_animator.SetTrigger(m_winAnimationHash);
	}
}

