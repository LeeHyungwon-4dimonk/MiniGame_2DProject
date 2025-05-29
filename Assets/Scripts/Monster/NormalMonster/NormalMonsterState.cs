using UnityEngine;

public class NormalMonsterState : BaseState
{
    protected NormalMonsterController m_slime;

    public NormalMonsterState(NormalMonsterController _slime)
    {
        m_slime = _slime;
    }

    public override void Enter()
    {

    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {

    }
}

public class NormalMonsterState_Idle : NormalMonsterState
{
    public NormalMonsterState_Idle(NormalMonsterController _slime) : base(_slime)
    {
        HasPhysics = false;
    }

    public override void Enter()
    {
        m_slime.IsMove = false;
        m_slime.Anim.Play(m_slime.IDLE_HASH);
        m_slime.Rigid.velocity = Vector2.zero;
        m_slime.Anim.SetBool("IsMove", m_slime.IsMove);
    }

    public override void Update()
    {
        base.Update();
        m_slime.IsMove = true;
        m_slime.StateMach.ChangeState(m_slime.StateMach.StateDic[EState.Walk]);
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
        m_slime.Anim.SetBool("IsMove", m_slime.IsMove);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        m_slime.Rigid.velocity = new Vector2(Vector2.right.x * m_slime.m_slimeData.MonsterMoveSpeed, m_slime.Rigid.velocity.y);
    }
}

public class NormalMonsterState_MeleeAttack : NormalMonsterState
{
    public NormalMonsterState_MeleeAttack(NormalMonsterController _slime) : base(_slime)
    {
        HasPhysics = false;
    }
}