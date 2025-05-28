using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerState : BaseState
{
    protected PlayerController m_player;

    public PlayerState(PlayerController _player)
    {
        m_player = _player;
    }

    public override void Enter() { }

    public override void Update()
    {
        if(m_player.IsJump && m_player.IsLand)
        {
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Jump]);
        }
    }

    public override void Exit() { }
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
        m_player.Anim.SetBool("IsMove", m_player.IsMove);
        m_player.Rigid.velocity = Vector2.zero;
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
}

public class Player_Jump : PlayerState
{
    public Player_Jump(PlayerController _player) : base(_player)
    {
        HasPhysics = true;
    }

    public override void Enter()
    {
        m_player.Anim.SetBool("IsJump", m_player.IsJump);
        m_player.Rigid.AddForce(Vector2.up * m_player.JumpPow, ForceMode2D.Impulse);
        m_player.IsLand = false;
    }

    public override void Update()
    {
        base.Update();
        if(m_player.IsLand)
        {
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Idle]);
            m_player.Anim.SetBool("IsJump", m_player.IsJump);
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
        
    }
}