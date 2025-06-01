using UnityEngine;

public class NormalMonsterState : BaseState
{
    protected NormalMonsterController m_slime;

    public NormalMonsterState(NormalMonsterController _slime)
    {
        m_slime = _slime;
    }

    public override void Enter() { }

    public override void Update()
    {
        // 쫒을 수 없는 상태면 탐지를 하지 않음
        if (!Trackable()) return;
        // 몬스터가 죽은 상태나 스턴 상태가 아니고, 쫒을 수 있는 상태면 플레이어를 추적함
        else if (!m_slime.IsDead && !m_slime.IsStun && Trackable())
            DetectPlayer();
    }

    public override void Exit() { }

    // 플레이어 추적
    private void DetectPlayer()
    {
        m_slime.Player = Physics2D.OverlapCircle(m_slime.transform.position, m_slime.m_slimeData.MonsterSight, m_slime.TargetLayer);

        // 플레이어가 존재, y축 범위 제한하여 바닥 밑/위 플레이어는 인식 못함, 쫓을 수 있는 상태일 때 추격 상태로 전환
        if (m_slime.Player != null
            && Mathf.Abs(m_slime.Player.transform.position.y -
            m_slime.transform.position.y) < 1f && Trackable())
        {
            m_slime.TargetTransform = m_slime.Player.transform;
            m_slime.StateMach.ChangeState(m_slime.StateMach.StateDic[EState.Trace]);
        }
    }
    // 쫓을 수 있는 상태인지 판정 - 앞에 절벽이 있으면 정지
    protected bool Trackable()
    {
        m_slime.PatrolVec = m_slime.SpriteRenderer.flipX == false ? Vector2.right : Vector2.left;

        Vector2 rayOrigin = m_slime.transform.position + new Vector3(m_slime.PatrolVec.x, 0);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.5f, m_slime.GroundLayer);
        Debug.DrawRay(rayOrigin, Vector2.down, Color.red, 0.01f);
        if (hit.collider == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}


public class NormalMonsterState_Idle : NormalMonsterState
{
    private float waitedTime;
    public NormalMonsterState_Idle(NormalMonsterController _slime) : base(_slime)
    {
        HasPhysics = false;
    }

    public override void Enter()
    {
        m_slime.IsMove = false;
        m_slime.Anim.SetBool("IsMove", m_slime.IsMove);
        m_slime.Anim.Play(m_slime.IDLE_HASH);
        m_slime.Rigid.velocity = Vector2.zero;
        if (m_slime.SpriteRenderer.flipX)
        {
            m_slime.SpriteRenderer.flipX = true;
            m_slime.PatrolVec = Vector2.left;
        }
        else
        {
            m_slime.SpriteRenderer.flipX = false;
            m_slime.PatrolVec = Vector2.right;
        }
        waitedTime = 0;
    }

    public override void Update()
    {
        base.Update();
        waitedTime += Time.deltaTime;
        if (waitedTime > 3)
        {
            m_slime.Rigid.velocity = Vector2.zero;
            m_slime.StateMach.ChangeState(m_slime.StateMach.StateDic[EState.Walk]);
        }
    }
}

public class NormalMonsterState_Walk : NormalMonsterState
{
    public NormalMonsterState_Walk(NormalMonsterController _slime) : base(_slime)
    {
        HasPhysics = true;
    }

    public override void Enter()
    {
        m_slime.IsMove = true;
        m_slime.Anim.SetBool("IsMove", m_slime.IsMove);
        if (m_slime.SpriteRenderer.flipX)
        {
            m_slime.SpriteRenderer.flipX = true;
            m_slime.PatrolVec = Vector2.left;
        }
        else
        {
            m_slime.SpriteRenderer.flipX = false;
            m_slime.PatrolVec = Vector2.right;
        }
    }

    public override void Update()
    {
        base.Update();
        Vector2 rayOrigin = m_slime.transform.position + new Vector3(m_slime.PatrolVec.x, 0);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.5f, m_slime.GroundLayer);
        if (hit.collider == null)
        {
            m_slime.SpriteRenderer.flipX = !m_slime.SpriteRenderer.flipX;
            m_slime.StateMach.ChangeState(m_slime.StateMach.StateDic[EState.Idle]);
        }
        Vector2 rayOrigin2 = m_slime.transform.position + new Vector3(m_slime.PatrolVec.x, 0);
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin2, m_slime.PatrolVec, 0.5f, m_slime.GroundLayer);
        if (!(hit2.collider == null))
        {
            m_slime.SpriteRenderer.flipX = !m_slime.SpriteRenderer.flipX;
            m_slime.StateMach.ChangeState(m_slime.StateMach.StateDic[EState.Idle]);
        }
    }

    public override void FixedUpdate()
    {
        if (Trackable())
            m_slime.Rigid.velocity = new Vector2(m_slime.PatrolVec.x * m_slime.m_slimeData.MonsterMoveSpeed, m_slime.Rigid.velocity.y);
    }
}

public class NormalMonsterState_Trace : NormalMonsterState
{
    Vector2 targetPos;
    public NormalMonsterState_Trace(NormalMonsterController _slime) : base(_slime)
    {
        HasPhysics = true;
    }
    public override void Enter()
    {
        m_slime.IsMove = true;
        m_slime.Anim.SetBool("IsMove", m_slime.IsMove);
    }

    public override void Update()
    {
        if (m_slime.Player == null || !Trackable())
        {
            m_slime.Rigid.velocity = Vector2.zero;
            m_slime.StateMach.ChangeState(m_slime.StateMach.StateDic[EState.Idle]);
        }

        targetPos = m_slime.TargetTransform.position;

        if (m_slime.TargetTransform.position.x > m_slime.transform.position.x)
        {
            m_slime.SpriteRenderer.flipX = false;
            targetPos = Vector2.right;
        }
        else
        {
            m_slime.SpriteRenderer.flipX = true;
            targetPos = Vector2.left;
        }

        if (Trackable() && Mathf.Abs(m_slime.TargetTransform.position.x - m_slime.transform.position.x) < 1.5f
            && Mathf.Abs(m_slime.TargetTransform.position.y - m_slime.transform.position.y) < 4f)
        {
            m_slime.StateMach.ChangeState(m_slime.StateMach.StateDic[EState.MeleeAttack]);
        }
    }

    public override void FixedUpdate()
    {
        if (Trackable())
            m_slime.Rigid.velocity = new Vector2(targetPos.x * m_slime.m_slimeData.MonsterMoveSpeed, m_slime.Rigid.velocity.y);
    }
}

public class NormalMonsterState_MeleeAttack : NormalMonsterState
{
    public NormalMonsterState_MeleeAttack(NormalMonsterController _slime) : base(_slime)
    {
        HasPhysics = false;
    }
    public override void Enter()
    {
        m_slime.Anim.Play(m_slime.MELEEATTACK_HASH);
    }

    public override void Update()
    {
        m_slime.Rigid.velocity = Vector2.zero;
        m_slime.AttackDelay += Time.deltaTime;
        if (m_slime.AttackDelay > 1.5f)
        {
            m_slime.AttackDelay = 0;
            m_slime.StateMach.ChangeState(m_slime.StateMach.StateDic[EState.Idle]);
        }
    }
}

public class NormalMonsterState_Stun : NormalMonsterState
{
    public NormalMonsterState_Stun(NormalMonsterController _slime) : base(_slime)
    {
        HasPhysics = false;
    }
    public override void Enter()
    {
        m_slime.Anim.Play(m_slime.IDLE_HASH);
        m_slime.IsStun = true;
    }

    public override void Update()
    {
        m_slime.Rigid.velocity = Vector2.zero;
        m_slime.StunDelay += Time.deltaTime;
        if (m_slime.StunDelay > 0.3f)
        {
            m_slime.StunDelay = 0;
            m_slime.IsStun = false;
            m_slime.StateMach.ChangeState(m_slime.StateMach.StateDic[EState.Idle]);
        }
    }
}

public class NormalMonsterState_Die : NormalMonsterState
{
    public NormalMonsterState_Die(NormalMonsterController _slime) : base(_slime)
    {
        HasPhysics = false;
    }
    public override void Enter()
    {
        m_slime.IsDead = true;
        m_slime.Anim.SetBool("IsDead", m_slime.IsDead);
        m_slime.Rigid.velocity = Vector2.zero;
        m_slime.Rigid.gravityScale = 0;
        CapsuleCollider2D collider2D;
        m_slime.TryGetComponent<CapsuleCollider2D>(out collider2D);
        if (collider2D != null) { collider2D.enabled = false; }
    }

    public override void Update()
    {
        base.Update();

        m_slime.DieDelay += Time.deltaTime;
        if (m_slime.DieDelay > 2f)
        {
            m_slime.ReturnPool();
        }
    }
}