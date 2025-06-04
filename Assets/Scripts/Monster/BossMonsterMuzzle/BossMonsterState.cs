using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BossMonsterState : BaseState
{
    protected BossMonsterController m_bossMonster;
    protected Transform m_targetTransform;

    public BossMonsterState(BossMonsterController _bossMonster)
    {
        m_bossMonster = _bossMonster;
    }

    public override void Enter() { }

    public override void Update() { }

    public override void Exit() { }
}

/// <summary>
/// ���� ��� ����
/// FixedUpdate : X
/// Transition -> �߰�
/// </summary>
public class BossMonsterState_Idle : BossMonsterState
{
    float m_delay;
    public BossMonsterState_Idle(BossMonsterController _bossMonster) : base(_bossMonster)
    {
        HasPhysics = false;
    }

    public override void Enter()
    {
        m_bossMonster.IsMove = false;
        m_bossMonster.Anim.SetBool("IsMove", m_bossMonster.IsMove);
        m_bossMonster.Anim.Play(m_bossMonster.IDLE_HASH);
        m_bossMonster.Rigid.velocity = Vector2.zero;
    }

    public override void Update()
    {
        m_delay += Time.deltaTime;
        if (m_delay > m_bossMonster.BossMobData.RestDelay)
        {
            m_delay = 0;
            m_bossMonster.StateMach.ChangeState(m_bossMonster.StateMach.StateDic[EState.Trace]);
        }
    }
}

/// <summary>
/// ���� �߰� ����
/// FixedUpdate : O
/// Transition : ���, ����
/// </summary>
public class BossMonsterState_Trace : BossMonsterState
{
    Vector2 targetPos;
    public BossMonsterState_Trace(BossMonsterController _bossMonster) : base(_bossMonster)
    {
        HasPhysics = true;
    }
    public override void Enter()
    {
        m_bossMonster.IsMove = true;
        m_bossMonster.Anim.SetBool("IsMove", m_bossMonster.IsMove);
    }

    public override void Update()
    {
        m_targetTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        // �÷��̾��� x��ǥ ��ġ�� �����Ͽ� �� ��ġ�� ����
        if (m_targetTransform.position.x > m_bossMonster.transform.position.x)
        {
            m_bossMonster.SpriteRenderer.flipX = true;
            targetPos = Vector2.right;
        }
        else
        {
            m_bossMonster.SpriteRenderer.flipX = false;
            targetPos = Vector2.left;
        }

        // �÷��̾ �ڽ��� ���ݹ��� ������ ������ �� ���� ���·� ��ȯ
        if (Mathf.Abs(m_targetTransform.position.x - m_bossMonster.transform.position.x) < m_bossMonster.BossMobData.AttackRange_X
            && Mathf.Abs(m_targetTransform.position.y - m_bossMonster.transform.position.y) < m_bossMonster.BossMobData.AttackRange_Y)
        {
            m_bossMonster.StateMach.ChangeState(m_bossMonster.StateMach.StateDic[EState.MeleeAttack]);
        }

    }

    public override void FixedUpdate()
    {
        m_bossMonster.Rigid.velocity = new Vector2(targetPos.x * m_bossMonster.BossMobData.MonsterMoveSpeed, m_bossMonster.Rigid.velocity.y);
    }
}

/// <summary>
/// ���� ���� ����
/// FixedUpdate : X
/// Transition : ���
/// </summary>
public class BossMonsterState_MeleeAttack : BossMonsterState
{
    public BossMonsterState_MeleeAttack(BossMonsterController _bossMonster) : base(_bossMonster)
    {
        HasPhysics = false;
    }
    public override void Enter()
    {        
        m_bossMonster.Anim.Play(m_bossMonster.MELEEATTACK_HASH);
    }

    public override void Update()
    {
        // ���Ͱ� �߰��ϸ鼭 �÷��̾ �����ϴ� ���������� ��� ����
        m_bossMonster.Rigid.velocity = Vector2.zero;

        // ���� �ִϸ��̼� ��½ð��� �ݿ��� ������ �ð�
        m_bossMonster.AttackDelay += Time.deltaTime;

        // ���� �ִϸ��̼� ��� �� ��� ���·� ��ȯ
        if (m_bossMonster.AttackDelay > m_bossMonster.BossMobData.MeleeAttackDelay)
        {
            m_bossMonster.AttackDelay = 0;
            m_bossMonster.StateMach.ChangeState(m_bossMonster.StateMach.StateDic[EState.Idle]);
        }
    }
}

/// <summary>
/// ���� ���� ����
/// FixedUpdate : X
/// Transition -> ���
/// �÷��̾ �����ϴ� ���߿��� ��� �������� ������ ���� ������
/// ���̵��� �ʹ� ���� ���� ����� �ο�
/// �뷱���� �Ϸ��� ���� �����̸� �������ָ� �Ǹ� ���� ��ġ�� 0.3f �� ����
/// </summary>
public class BossMonsterState_Stun : BossMonsterState
{
    public BossMonsterState_Stun(BossMonsterController _bossMonster) : base(_bossMonster)
    {
        HasPhysics = false;
    }
    public override void Enter()
    {
        m_bossMonster.Anim.Play(m_bossMonster.IDLE_HASH);
        m_bossMonster.IsStun = true;
    }

    public override void Update()
    {
        m_bossMonster.Rigid.velocity = Vector2.zero;
        m_bossMonster.StunDelay += Time.deltaTime;

        // ���� �����̸� �����Ͽ� �뷱��
        if (m_bossMonster.StunDelay > m_bossMonster.BossMobData.StunCoolTime)
        {
            m_bossMonster.StunDelay = 0;
            m_bossMonster.IsStun = false;
            m_bossMonster.StateMach.ChangeState(m_bossMonster.StateMach.StateDic[EState.Trace]);
        }
    }
}

/// <summary>
/// ���� ���� ����
/// FixedUpdate : X
/// Transition X, ���ʹ� ��Ȱ��ȭ
/// </summary>
public class BossMonsterState_Die : BossMonsterState
{
    public BossMonsterState_Die(BossMonsterController _bossMonster) : base(_bossMonster)
    {
        HasPhysics = false;
    }
    public override void Enter()
    {
        m_bossMonster.IsDead = true;
        m_bossMonster.Anim.SetBool("IsDead", m_bossMonster.IsDead);
        m_bossMonster.Rigid.velocity = Vector2.zero;
        m_bossMonster.Rigid.gravityScale = 0;
        CapsuleCollider2D collider2D;
        m_bossMonster.TryGetComponent<CapsuleCollider2D>(out collider2D);
        if (collider2D != null) { collider2D.enabled = false; }
    }

    public override void Update()
    {
        base.Update();

        m_bossMonster.DieDelay += Time.deltaTime;
        if (m_bossMonster.DieDelay > m_bossMonster.BossMobData.DieDelay)
        {
            m_bossMonster.gameObject.SetActive(false);
        }
    }
}