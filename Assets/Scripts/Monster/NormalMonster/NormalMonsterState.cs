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
    /// �ǽð� ������Ʈ ���� ����
    /// * �߰�
    /// </summary>
    public override void Update()
    {
        // �i�� �� ���� ���¸� Ž���� ���� ����
        if (!Trackable()) return;
        // ���Ͱ� ���� ���³� ���� ���°� �ƴϰ�, �i�� �� �ִ� ���¸� �÷��̾ ������
        else if (!m_normalMonster.IsDead && !m_normalMonster.IsStun && Trackable())
            DetectPlayer();
    }

    public override void Exit() { }

    // �÷��̾� ����
    private void DetectPlayer()
    {
        m_normalMonster.Player = Physics2D.OverlapCircle(m_normalMonster.transform.position, m_normalMonster.NormalMobData.MonsterSight, m_normalMonster.TargetLayer);

        // �÷��̾ ����, y�� ���� �����Ͽ� �ٴ� ��/�� �÷��̾�� �ν� ����
        // ���� �� �ִ� ������ �� �߰� ���·� ��ȯ
        if (m_normalMonster.Player != null
            && Mathf.Abs(m_normalMonster.Player.transform.position.y -
            m_normalMonster.transform.position.y) < 1f && Trackable())
        {
            m_normalMonster.TargetTransform = m_normalMonster.Player.transform;
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.Trace]);
        }
    }

    // ���� ���� �������� ���� - �տ� �����̳� ���� ������ ����
    protected bool Trackable()
    {
        m_normalMonster.PatrolVec = m_normalMonster.SpriteRenderer.flipX == false ? Vector2.right : Vector2.left;

        // ������ �ִ��� ����
        Vector2 rayOrigin = m_normalMonster.transform.position + new Vector3(m_normalMonster.PatrolVec.x, 0);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.5f, m_normalMonster.GroundLayer);
        Debug.DrawRay(rayOrigin, Vector2.down, Color.red, 0.01f);
        if (hit.collider == null)
        {
            return false;
        }
        else
        {
            // �տ� �Ѿ �� ���� ���� �־� �� �̻� �߰� �Ұ����� ����
            Vector2 rayOrigin2 = m_normalMonster.transform.position + new Vector3(m_normalMonster.PatrolVec.x, 0);
            RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin2, m_normalMonster.PatrolVec, 0.5f, m_normalMonster.GroundLayer);
            if (!(hit2.collider == null))
            {
                return false;
            }
            return true;
        }
    }

    // �ߺ� �ڵ带 �θ� ������Ʈ�� �Űܼ� ���( Idle, Walk���� ��� )
    // ������ Ž�� ���̴��� �ִϸ��̼��� Flip�ϴ� ���
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
/// ���� ��� ����
/// FixedUpdate : X
/// Transition -> �ȱ�
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

        // ���� ��ȯ
        Flip();
        waitedTime = 0;
    }

    public override void Update()
    {
        base.Update();
        waitedTime += Time.deltaTime;
        // 3�� ��� �� �ȱ�� ��ȯ
        if (waitedTime > 3)
        {
            m_normalMonster.Rigid.velocity = Vector2.zero;
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.Walk]);
        }
    }
}

/// <summary>
/// ���� �ȱ� ����
/// FixedUpdate : O
/// Transition -> ���
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

        // ���� ��ȯ
        Flip();
    }

    public override void Update()
    {
        base.Update();

        // �տ� ������ �ִ��� �����ϰ�, ������ ��� ���·� ��ȯ�� ������ ��ȯ�� 
        Vector2 rayOrigin = m_normalMonster.transform.position + new Vector3(m_normalMonster.PatrolVec.x, 0);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.5f, m_normalMonster.GroundLayer);
        if (hit.collider == null)
        {
            m_normalMonster.SpriteRenderer.flipX = !m_normalMonster.SpriteRenderer.flipX;
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.Idle]);
        }

        // �տ� �ö� �� ���� ���� �ִ��� �����ϰ�, ������ ��� ���·� ��ȯ�� ������ ��ȯ��
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
/// ���� �߰� ����
/// FixedUpdate : O
/// Transition : ���, ����
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
        // �÷��̾ ��ġ�ų� �߰� �Ұ����� ������ ��� ��� ���·� ��ȯ
        if (m_normalMonster.Player == null || !Trackable())
        {
            m_normalMonster.Rigid.velocity = Vector2.zero;
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.Idle]);
        }

        targetPos = m_normalMonster.TargetTransform.position;

        // �÷��̾��� x��ǥ ��ġ�� �����Ͽ� �� ��ġ�� ����
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

        // �÷��̾ �ڽ��� ���ݹ��� ������ ������ �� ���� ���·� ��ȯ
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
/// ���� ���� ����
/// FixedUpdate : X
/// Transition : ���
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
        // ���Ͱ� �߰��ϸ鼭 �÷��̾ �����ϴ� ���������� ��� ����
        m_normalMonster.Rigid.velocity = Vector2.zero;

        // ���� �ִϸ��̼� ��½ð��� �ݿ��� ������ �ð�
        m_normalMonster.AttackDelay += Time.deltaTime;

        // ���� �ִϸ��̼� ��� �� ��� ���·� ��ȯ
        if (m_normalMonster.AttackDelay > 1.5f)
        {
            m_normalMonster.AttackDelay = 0;
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.Idle]);
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

        // ���� �����̸� �����Ͽ� �뷱��
        if (m_normalMonster.StunDelay > 0.3f)
        {
            m_normalMonster.StunDelay = 0;
            m_normalMonster.IsStun = false;
            m_normalMonster.StateMach.ChangeState(m_normalMonster.StateMach.StateDic[EState.Idle]);
        }
    }
}

/// <summary>
/// ���� ���� ����
/// FixedUpdate : X
/// Transition X, ���ʹ� Ǯ�� ���ư�(��Ȱ��ȭ)
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