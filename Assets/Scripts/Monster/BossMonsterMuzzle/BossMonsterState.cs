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
/// 몬스터 대기 상태
/// FixedUpdate : X
/// Transition -> 추격
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
/// 몬스터 추격 상태
/// FixedUpdate : O
/// Transition : 대기, 공격
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
        // 플레이어의 x좌표 위치를 판정하여 그 위치로 향함
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

        // 플레이어가 자신의 공격범위 안으로 들어왔을 때 공격 상태로 전환
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
/// 몬스터 공격 상태
/// FixedUpdate : X
/// Transition : 대기
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
        // 몬스터가 추격하면서 플레이어를 공격하는 비정상적인 출력 방지
        m_bossMonster.Rigid.velocity = Vector2.zero;

        // 공격 애니메이션 출력시간을 반영한 딜레이 시간
        m_bossMonster.AttackDelay += Time.deltaTime;

        // 공격 애니메이션 출력 후 대기 상태로 전환
        if (m_bossMonster.AttackDelay > m_bossMonster.BossMobData.MeleeAttackDelay)
        {
            m_bossMonster.AttackDelay = 0;
            m_bossMonster.StateMach.ChangeState(m_bossMonster.StateMach.StateDic[EState.Idle]);
        }
    }
}

/// <summary>
/// 몬스터 스턴 상태
/// FixedUpdate : X
/// Transition -> 대기
/// 플레이어가 공격하는 와중에도 계속 데미지가 들어오는 구조 때문에
/// 난이도가 너무 높아 스턴 기능을 부여
/// 밸런싱을 하려면 스턴 딜레이를 조정해주면 되며 현재 수치는 0.3f 로 설정
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

        // 스턴 딜레이를 조정하여 밸런싱
        if (m_bossMonster.StunDelay > m_bossMonster.BossMobData.StunCoolTime)
        {
            m_bossMonster.StunDelay = 0;
            m_bossMonster.IsStun = false;
            m_bossMonster.StateMach.ChangeState(m_bossMonster.StateMach.StateDic[EState.Trace]);
        }
    }
}

/// <summary>
/// 몬스터 죽음 상태
/// FixedUpdate : X
/// Transition X, 몬스터는 비활성화
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