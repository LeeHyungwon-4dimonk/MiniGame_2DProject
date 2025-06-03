using UnityEngine;

public class NormalMonsterState : BaseState
{
    protected NormalMonsterController m_normalMonster;

    public NormalMonsterState(NormalMonsterController _normalMonster)
    {
        m_normalMonster = _normalMonster;
    }

    public override void Enter() { }

    /// <summary>
    /// 실시간 업데이트 가능 상태
    /// * 추격
    /// </summary>
    public override void Update()
    {
        // 쫒을 수 없는 상태면 탐지를 하지 않음
        if (!Trackable()) return;
        // 몬스터가 죽은 상태나 스턴 상태가 아니고, 쫒을 수 있는 상태면 플레이어를 추적함
        else if (!m_normalMonster.IsDead && !m_normalMonster.IsStun && Trackable())
            DetectPlayer();
    }

    public override void Exit() { }

    // 플레이어 추적
    private void DetectPlayer()
    {
        m_normalMonster.Player = Physics2D.OverlapCircle(m_normalMonster.transform.position, m_normalMonster.NormalMobData.MonsterSight, m_normalMonster.TargetLayer);

        // 플레이어가 존재, y축 범위 제한하여 바닥 밑/위 플레이어는 인식 못함
        // 쫓을 수 있는 상태일 때 추격 상태로 전환
        if (m_normalMonster.Player != null
            && Mathf.Abs(m_normalMonster.Player.transform.position.y -
            m_normalMonster.transform.position.y) < 1f && Trackable())
        {
            m_normalMonster.TargetTransform = m_normalMonster.Player.transform;
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.Trace]);
        }
    }

    // 추적 가능 상태인지 판정 - 앞에 절벽이나 벽이 있으면 정지
    protected bool Trackable()
    {
        m_normalMonster.PatrolVec = m_normalMonster.SpriteRenderer.flipX == false ? Vector2.right : Vector2.left;

        // 절벽이 있는지 판정
        Vector2 rayOrigin = m_normalMonster.transform.position + new Vector3(m_normalMonster.PatrolVec.x, 0);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.5f, m_normalMonster.GroundLayer);
        Debug.DrawRay(rayOrigin, Vector2.down, Color.red, 0.01f);
        if (hit.collider == null)
        {
            return false;
        }
        else
        {
            // 앞에 넘어갈 수 없는 벽이 있어 더 이상 추격 불가한지 판정
            Vector2 rayOrigin2 = m_normalMonster.transform.position + new Vector3(m_normalMonster.PatrolVec.x, 0);
            RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin2, m_normalMonster.PatrolVec, 0.5f, m_normalMonster.GroundLayer);
            if (!(hit2.collider == null))
            {
                return false;
            }
            return true;
        }
    }

    // 중복 코드를 부모 오브젝트로 옮겨서 상속( Idle, Walk에서 사용 )
    // 몬스터의 탐지 레이더와 애니메이션을 Flip하는 기능
    protected void Flip()
    {
        if (m_normalMonster.SpriteRenderer.flipX)
        {
            m_normalMonster.SpriteRenderer.flipX = true;
            m_normalMonster.PatrolVec = Vector2.left;
        }
        else
        {
            m_normalMonster.SpriteRenderer.flipX = false;
            m_normalMonster.PatrolVec = Vector2.right;
        }
    }
}

/// <summary>
/// 몬스터 대기 상태
/// FixedUpdate : X
/// Transition -> 걷기
/// </summary>
public class NormalMonsterState_Idle : NormalMonsterState
{
    private float waitedTime;
    public NormalMonsterState_Idle(NormalMonsterController _normalMonster) : base(_normalMonster)
    {
        HasPhysics = false;
    }

    public override void Enter()
    {
        m_normalMonster.IsMove = false;
        m_normalMonster.Anim.SetBool("IsMove", m_normalMonster.IsMove);
        m_normalMonster.Anim.Play(m_normalMonster.IDLE_HASH);
        m_normalMonster.Rigid.velocity = Vector2.zero;

        // 방향 전환
        Flip();
        waitedTime = 0;
    }

    public override void Update()
    {
        base.Update();
        waitedTime += Time.deltaTime;
        // 3초 대기 후 걷기로 전환
        if (waitedTime > 3)
        {
            m_normalMonster.Rigid.velocity = Vector2.zero;
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.Walk]);
        }
    }
}

/// <summary>
/// 몬스터 걷기 상태
/// FixedUpdate : O
/// Transition -> 대기
/// </summary>
public class NormalMonsterState_Walk : NormalMonsterState
{
    public NormalMonsterState_Walk(NormalMonsterController _normalMonster) : base(_normalMonster)
    {
        HasPhysics = true;
    }

    public override void Enter()
    {
        m_normalMonster.IsMove = true;
        m_normalMonster.Anim.SetBool("IsMove", m_normalMonster.IsMove);

        // 방향 전환
        Flip();
    }

    public override void Update()
    {
        base.Update();

        // 앞에 절벽이 있는지 판정하고, 있으면 대기 상태로 전환해 방향을 전환함 
        Vector2 rayOrigin = m_normalMonster.transform.position + new Vector3(m_normalMonster.PatrolVec.x, 0);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.5f, m_normalMonster.GroundLayer);
        if (hit.collider == null)
        {
            m_normalMonster.SpriteRenderer.flipX = !m_normalMonster.SpriteRenderer.flipX;
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.Idle]);
        }

        // 앞에 올라갈 수 없는 벽이 있는지 판정하고, 있으면 대기 상태로 전환해 방향을 전환함
        Vector2 rayOrigin2 = m_normalMonster.transform.position + new Vector3(m_normalMonster.PatrolVec.x, 0);
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin2, m_normalMonster.PatrolVec, 0.5f, m_normalMonster.GroundLayer);
        if (!(hit2.collider == null))
        {
            m_normalMonster.SpriteRenderer.flipX = !m_normalMonster.SpriteRenderer.flipX;
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.Idle]);
        }
    }

    public override void FixedUpdate()
    {
        if (Trackable())
            m_normalMonster.Rigid.velocity = new Vector2(m_normalMonster.PatrolVec.x * m_normalMonster.NormalMobData.MonsterMoveSpeed, m_normalMonster.Rigid.velocity.y);
    }
}

/// <summary>
/// 몬스터 추격 상태
/// FixedUpdate : O
/// Transition : 대기, 공격
/// </summary>
public class NormalMonsterState_Trace : NormalMonsterState
{
    Vector2 targetPos;
    public NormalMonsterState_Trace(NormalMonsterController _normalMonster) : base(_normalMonster)
    {
        HasPhysics = true;
    }
    public override void Enter()
    {
        m_normalMonster.IsMove = true;
        m_normalMonster.Anim.SetBool("IsMove", m_normalMonster.IsMove);
    }

    public override void Update()
    {
        // 플레이어를 놓치거나 추격 불가능한 상태인 경우 대기 상태로 전환
        if (m_normalMonster.Player == null || !Trackable())
        {
            m_normalMonster.Rigid.velocity = Vector2.zero;
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.Idle]);
        }

        targetPos = m_normalMonster.TargetTransform.position;

        // 플레이어의 x좌표 위치를 판정하여 그 위치로 향함
        if (m_normalMonster.TargetTransform.position.x > m_normalMonster.transform.position.x)
        {
            m_normalMonster.SpriteRenderer.flipX = false;
            targetPos = Vector2.right;
        }
        else
        {
            m_normalMonster.SpriteRenderer.flipX = true;
            targetPos = Vector2.left;
        }

        // 플레이어가 자신의 공격범위 안으로 들어왔을 때 공격 상태로 전환
        if (Trackable() && Mathf.Abs(m_normalMonster.TargetTransform.position.x - m_normalMonster.transform.position.x) < 1.5f
            && Mathf.Abs(m_normalMonster.TargetTransform.position.y - m_normalMonster.transform.position.y) < 4f)
        {
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.MeleeAttack]);
        }
    }

    public override void FixedUpdate()
    {
        if (Trackable())
            m_normalMonster.Rigid.velocity = new Vector2(targetPos.x * m_normalMonster.NormalMobData.MonsterMoveSpeed, m_normalMonster.Rigid.velocity.y);
    }
}

/// <summary>
/// 몬스터 공격 상태
/// FixedUpdate : X
/// Transition : 대기
/// </summary>
public class NormalMonsterState_MeleeAttack : NormalMonsterState
{
    public NormalMonsterState_MeleeAttack(NormalMonsterController _normalMonster) : base(_normalMonster)
    {
        HasPhysics = false;
    }
    public override void Enter()
    {
        m_normalMonster.Anim.Play(m_normalMonster.MELEEATTACK_HASH);
    }

    public override void Update()
    {
        // 몬스터가 추격하면서 플레이어를 공격하는 비정상적인 출력 방지
        m_normalMonster.Rigid.velocity = Vector2.zero;

        // 공격 애니메이션 출력시간을 반영한 딜레이 시간
        m_normalMonster.AttackDelay += Time.deltaTime;

        // 공격 애니메이션 출력 후 대기 상태로 전환
        if (m_normalMonster.AttackDelay > 1.5f)
        {
            m_normalMonster.AttackDelay = 0;
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.Idle]);
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
public class NormalMonsterState_Stun : NormalMonsterState
{
    public NormalMonsterState_Stun(NormalMonsterController _normalMonster) : base(_normalMonster)
    {
        HasPhysics = false;
    }
    public override void Enter()
    {
        m_normalMonster.Anim.Play(m_normalMonster.IDLE_HASH);
        m_normalMonster.IsStun = true;
    }

    public override void Update()
    {
        m_normalMonster.Rigid.velocity = Vector2.zero;
        m_normalMonster.StunDelay += Time.deltaTime;

        // 스턴 딜레이를 조정하여 밸런싱
        if (m_normalMonster.StunDelay > 0.3f)
        {
            m_normalMonster.StunDelay = 0;
            m_normalMonster.IsStun = false;
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.Idle]);
        }
    }
}

/// <summary>
/// 몬스터 죽음 상태
/// FixedUpdate : X
/// Transition X, 몬스터는 풀로 돌아감(비활성화)
/// </summary>
public class NormalMonsterState_Die : NormalMonsterState
{
    public NormalMonsterState_Die(NormalMonsterController _normalMonster) : base(_normalMonster)
    {
        HasPhysics = false;
    }
    public override void Enter()
    {
        m_normalMonster.IsDead = true;
        m_normalMonster.Anim.SetBool("IsDead", m_normalMonster.IsDead);
        m_normalMonster.Rigid.velocity = Vector2.zero;
        m_normalMonster.Rigid.gravityScale = 0;
        CapsuleCollider2D collider2D;
        m_normalMonster.TryGetComponent<CapsuleCollider2D>(out collider2D);
        if (collider2D != null) { collider2D.enabled = false; }
    }

    public override void Update()
    {
        base.Update();

        m_normalMonster.DieDelay += Time.deltaTime;
        if (m_normalMonster.DieDelay > 2f)
        {
            m_normalMonster.ReturnPool();
        }
    }
}