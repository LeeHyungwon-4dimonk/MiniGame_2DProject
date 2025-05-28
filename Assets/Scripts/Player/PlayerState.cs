using UnityEngine;


public class PlayerState : BaseState
{
    protected PlayerController m_player;

    public PlayerState(PlayerController _player)
    {
        m_player = _player;
    }

    public override void Enter()
    {

    }

    public override void Update()
    {
        if (!m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_ChargeSpell")
            && !m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_SpellAttack")
            && m_player.MeleeAttackAction.IsPressed())
        {
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.MeleeAttack]);
        }

        if(!m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_MeleeAttack")
            && m_player.RangeAttackAction.IsPressed())
        {
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Charge]);
        }        

        if (!m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_MeleeAttack")
            && !m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_ChargeSpell")
            && !m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_SpellAttack")
            && m_player.JumpAction.IsPressed() && m_player.IsLand)
        {
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Jump]);
        }        
    }

    public override void Exit()
    {
        m_player.Anim.SetBool("IsMove", m_player.IsMove);
        m_player.Anim.SetBool("IsJump", m_player.IsJump);
    }
}

public class Player_Idle : PlayerState
{
    public Player_Idle(PlayerController _player) : base(_player)
    {
        HasPhysics = false;
    }

    public override void Enter()
    {
        m_player.IsMove = false;
        m_player.Anim.Play(m_player.IDLE_HASH);
        m_player.Rigid.velocity = Vector2.zero;
        m_player.Anim.SetBool("IsMove", m_player.IsMove);
    }

    public override void Update()
    {
        base.Update();
        if(Mathf.Abs(m_player.InputX.x) > 0.1f)
        {
            m_player.IsMove = true;
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Walk]);            
        }       
    }
}

public class Player_Walk : PlayerState
{
    public Player_Walk(PlayerController _player) : base(_player)
    {
        HasPhysics = true;
    }

    public override void Enter()
    {
        m_player.Anim.SetBool("IsMove", m_player.IsMove);
    }

    public override void Update()
    {
        base.Update();
        if(Mathf.Abs(m_player.InputX.x) < 0.1f)
        {
            m_player.IsMove = false;
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Idle]);            
        }

        if(m_player.InputX.x < 0)
        {
            m_player.SpriteRenderer.flipX = true;
        }
        else if(m_player.InputX.x > 0)
        {
            m_player.SpriteRenderer.flipX = false;
        }        
    }

    public override void FixedUpdate()
    {
        m_player.Rigid.velocity = new Vector2(m_player.InputX.x * m_player.MoveSpeed, m_player.Rigid.velocity.y);
    }

    public override void Exit()
    {
        m_player.Anim.SetBool("IsMove", m_player.IsMove);
    }
}

public class Player_Jump : PlayerState
{
    public Player_Jump(PlayerController _player) : base(_player)
    {
        HasPhysics = true;
    }

    public override void Enter()
    {
        m_player.IsLand = false; 
        m_player.IsJump = true;
        m_player.Anim.SetBool("IsJump", m_player.IsJump);
        m_player.Rigid.AddForce(Vector2.up * m_player.JumpPow, ForceMode2D.Impulse);
    }

    public override void Update()
    {
        base.Update();
        if(m_player.IsLand)
        {
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Idle]);
        }

        if (m_player.InputX.x < 0)
        {
            m_player.SpriteRenderer.flipX = true;
        }
        else if (m_player.InputX.x > 0)
        {
            m_player.SpriteRenderer.flipX = false;
        }
    }

    public override void FixedUpdate()
    {
        m_player.Rigid.velocity = new Vector2(m_player.InputX.x * m_player.MoveSpeed, m_player.Rigid.velocity.y);
    }
    public override void Exit()
    {
        m_player.Anim.SetBool("IsJump", m_player.IsJump);
    }
}

public class Player_MeleeAttack : PlayerState
{
    public Player_MeleeAttack(PlayerController _player) : base(_player)
    {
        HasPhysics = false;
    }
    public override void Enter()
    {
        m_player.Anim.Play(m_player.MELEEATTACK_HASH);
        m_player.IsAim = true;
    }

    public override void Update()
    {
        base.Update();
        m_player.MeleeAttackCoolTime += Time.deltaTime;
        if (m_player.MeleeAttackCoolTime > 0.8)
        {
            m_player.MeleeAttackCoolTime = 0;
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Idle]);            
        }
    }
}

public class Player_Charge : PlayerState
{
    public Player_Charge(PlayerController _player) : base(_player)
    {
        HasPhysics = false;
    }
    public override void Enter()
    {
        m_player.IsAim = true;
        m_player.Anim.Play(m_player.CHARGE_HASH);
    }

    public override void Update()
    {
        base.Update();
        if (m_player.IsAim && m_player.RangeAttackAction.WasReleasedThisFrame())
        {
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.RangedAttack]);
        }
    }
}

public class Player_RangeAttack : PlayerState
{
    public Player_RangeAttack(PlayerController _player) : base(_player)
    {
        HasPhysics = false;
    }

    public override void Enter()
    {
        m_player.Muzzle.Fire();
        m_player.Anim.Play(m_player.SPELLATTACK_HASH);                
    }

    public override void Update()
    {
        base.Update();
        m_player.RangeAttackCoolTime += Time.deltaTime;
        if (m_player.RangeAttackCoolTime > 0.4f)
        {            
            m_player.IsAim = false;
            m_player.RangeAttackCoolTime = 0;
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Idle]);
        }
    }
}