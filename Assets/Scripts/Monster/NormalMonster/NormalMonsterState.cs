using Unity.VisualScripting;
using UnityEngine;

public class NormalMonsterState : BaseState
{
    protected NormalMonsterController m_slime;

    public NormalMonsterState(NormalMonsterController _slime)
    {
        m_slime = _slime;
    }

    public override void Enter() { }

    public override void Update() { }

    public override void Exit() { }
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
        m_slime.SpriteRenderer.flipX = !m_slime.SpriteRenderer.flipX;
        if(m_slime.SpriteRenderer.flipX)
        {
            m_slime.PatrolVec = Vector2.left;
        }
        else
        {
            m_slime.PatrolVec = Vector2.right;
        }
        waitedTime = 0;
    }

    public override void Update()
    {
        waitedTime += Time.deltaTime;
        if (waitedTime > 3)
        {
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
    }

    public override void Update()
    {
        base.Update();
        Vector2 rayOrigin = m_slime.transform.position + new Vector3(m_slime.PatrolVec.x, 0);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down,3f,m_slime.GroundLayer);
        if (hit.collider == null) 
        {
            m_slime.StateMach.ChangeState(m_slime.StateMach.StateDic[EState.Idle]);
        }
        
        Vector2 rayOrigin2 = m_slime.transform.position + new Vector3(m_slime.PatrolVec.x, -1);
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin2, m_slime.PatrolVec, 0.5f, m_slime.GroundLayer);
        if (!(hit2.collider == null))
        {
            m_slime.StateMach.ChangeState(m_slime.StateMach.StateDic[EState.Idle]);
        }
        
    }

    public override void FixedUpdate()
    {
        m_slime.Rigid.velocity = m_slime.PatrolVec * m_slime.m_slimeData.MonsterMoveSpeed;
    }
}

public class NormalMonsterState_MeleeAttack : NormalMonsterState
{
    public NormalMonsterState_MeleeAttack(NormalMonsterController _slime) : base(_slime)
    {
        HasPhysics = false;
    }
}