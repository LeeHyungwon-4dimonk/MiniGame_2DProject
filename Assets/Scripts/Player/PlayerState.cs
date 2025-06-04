using UnityEngine;

public class PlayerState : BaseState
{
    protected PlayerController m_player;

    public PlayerState(PlayerController _player)
    {
        m_player = _player;
    }

    public override void Enter() { }

    /// <summary>
    /// 실시간 업데이트 가능 상태
    /// * 근거리 공격
    /// * 차지
    /// * 점프
    /// </summary>
    public override void Update()
    {
        // 플레이어가 차징 상태나 스펠 공격 발사 중이 아닐 때
        // 플레이어의 스펠 쿨타임이 감소
        if(m_player.StateMach.CurState != m_player.StateMach.StateDic[EState.Charge]
            && m_player.StateMach.CurState != m_player.StateMach.StateDic[EState.RangedAttack])
        {
            GameManager.Instance.SetCoolTime();
        }

        // 플레이어가 차징 상태나 스펠 공격 발사중이 아닐 때
        // 플레이어의 마우스가 일시정지 버튼에 올라가 있는 것이 아닐 때
        // 플레이어는 마우스 클릭을 했을 때 근접 공격을 할 수 있음
        // (모션 캔슬 방지용)
        if (!m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_ChargeSpell")
            && !m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_SpellAttack")
            && m_player.MeleeAttackAction.IsPressed() && !GameManager.Instance.IsTryPause())
        {
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.MeleeAttack]);
        }

        // 플레이어가 근접 공격 중이 아닐 때
        // 플레이어가 마나가 있고 쿨타임이 다 되었을 때
        // 플레이어는 마우스 우클릭을 했을 때 스펠 공격을 할 수 있음
        if(!m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_MeleeAttack")
            && m_player.RangeAttackAction.IsPressed() && GameManager.Instance.GetCoolTime() < 0
            && GameManager.Instance.GetCurMP() > 0)
        {
            GameManager.Instance.SetCoolTime(3);
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Charge]);
        }

        // 플레이어가 근접 공격 중이 아닐 때
        // 플레이어가 차징 상태나 스펠 공격 발사중이 아닐 때
        // 플레이어가 점프 가능한 상태일 때
        // 플레이어는 스페이스바를 눌렀을 때 점프를 할 수 있음
        // (모션 캔슬 방지용)
        if (!m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_MeleeAttack")
            && !m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_ChargeSpell")
            && !m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_SpellAttack")
            && m_player.JumpAction.IsPressed() && m_player.IsLand)
        {
            m_player.SFXCtrl.StopSFX();
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Jump]);
        }        
    }

    public override void Exit()
    {
        m_player.Anim.SetBool("IsMove", m_player.IsMove);
        m_player.Anim.SetBool("IsJump", m_player.IsJump);
    }

    // 중복 코드를 부모 오브젝트로 옮겨서 상속 ( Walk, Jump에서 사용 )
    // 플레이어의 입력에 따라 애니메이션을 Flip, 충돌체를 Flip하는 기능
    protected void Flip()
    {
        if (m_player.InputX.x < 0)
        {
            m_player.SpriteRenderer.flipX = true;
            m_player.PlayerCollider.offset = new Vector2(-0.4f, 0.8f);
        }
        else if (m_player.InputX.x > 0)
        {
            m_player.SpriteRenderer.flipX = false;
            m_player.PlayerCollider.offset = new Vector2(0.4f, 0.8f);
        }
    }
}

/// <summary>
/// 플레이어 대기 상태
/// FixedUpdate : X
/// Transition -> 걷기
/// </summary>
public class Player_Idle : PlayerState
{
    public Player_Idle(PlayerController _player) : base(_player)
    {
        HasPhysics = false;
    }

    public override void Enter()
    {
        m_player.Rigid.velocity = Vector2.zero;
        m_player.IsMove = false;
        m_player.Anim.SetBool("IsMove", m_player.IsMove);
        m_player.Anim.Play(m_player.IDLE_HASH);
    }

    public override void Update()
    {
        base.Update();

        // Input 입력값이 있을 때 걷기 상태로 전환
        if(Mathf.Abs(m_player.InputX.x) > 0.1f)
        {
            m_player.IsMove = true;
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Walk]);            
        }       
    }
}

/// <summary>
/// 플레이어 걷기 상태
/// FixedUpdate : O
/// Transition -> 대기
/// </summary>
public class Player_Walk : PlayerState
{
    public Player_Walk(PlayerController _player) : base(_player)
    {
        HasPhysics = true;
    }

    public override void Enter()
    {
        m_player.Anim.SetBool("IsMove", m_player.IsMove);
        m_player.SFXCtrl.LoopSFX("FootStep");
    }

    public override void Update()
    {
        base.Update();

        // Input 입력값이 없을 때 대기 상태로 전환
        if(Mathf.Abs(m_player.InputX.x) < 0.1f)
        {
            m_player.IsMove = false;
            m_player.SFXCtrl.StopSFX();
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Idle]);            
        }

        // Input 입력값이 있을 때 플레이어의 애니메이션 출력 방향 및 충돌체 방향 전환
        Flip();
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

/// <summary>
/// 플레이어 점프 상태
/// FixedUpdate : O
/// Transition -> 대기
/// </summary>
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
        m_player.SFXCtrl.PlaySFX("Jump");
        m_player.Rigid.AddForce(Vector2.up * m_player.JumpPow, ForceMode2D.Impulse);
    }

    public override void Update()
    {
        base.Update();
        
        // 플레이어가 바닥으로 착지했을 때 대기 상태로 전환
        if(m_player.IsLand)
        {
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Idle]);
        }

        // Input 입력값이 있을 때 플레이어의 애니메이션 출력 방향 및 충돌체 방향 전환 
        Flip();
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

/// <summary>
/// 플레이어 근접 공격 상태
/// FixedUpdate : X
/// Transition : 대기
/// </summary>
public class Player_MeleeAttack : PlayerState
{
    public Player_MeleeAttack(PlayerController _player) : base(_player)
    {
        HasPhysics = false;
    }
    public override void Enter()
    {
        m_player.Anim.Play(m_player.MELEEATTACK_HASH);
        m_player.SFXCtrl.PlaySFX("Sword");
    }

    public override void Update()
    {
        base.Update();
        // 공격 애니메이션 출력 시간을 반영한 딜레이 시간
        m_player.MeleeAttackDelay += Time.deltaTime;

        // 공격 애니메이션 출력 후 대기 상태로 전환
        if (m_player.MeleeAttackDelay > 0.8)
        {
            m_player.MeleeAttackDelay = 0;
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Idle]);            
        }
    }
}

/// <summary>
/// 플레이어 차지 상태
/// FixedUpdate : X
/// Transition -> 스펠 공격
/// 현 단계에서 차지를 캔슬할 방법이 구현되지 않음(TODO)
/// </summary>
public class Player_Charge : PlayerState
{
    public Player_Charge(PlayerController _player) : base(_player)
    {
        HasPhysics = false;
    }
    public override void Enter()
    {
        m_player.Anim.Play(m_player.CHARGE_HASH);
        m_player.SFXCtrl.LoopSFX("Charge");
        m_player.ChargeTime = 0;
        m_player.IsCharging = true;
    }

    public override void Update()
    {
        base.Update();
        // 플레이어의 차지 시간에 따른 데미지 반영을 위한 수치 반영
        m_player.ChargeTime += Time.deltaTime;

        // 플레이어가 마우스클릭 상태에세 키를 뗐을 때 스펠 공격 상태로 전환
        if (m_player.RangeAttackAction.WasReleasedThisFrame())
        {
            m_player.SFXCtrl.StopSFX();
            m_player.IsCharging = false;
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.RangedAttack]);
        }
    }
}

/// <summary>
/// 플레이어 스펠 공격
/// FixedUpdate: X
/// Transition -> 대기
/// </summary>
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
        m_player.SFXCtrl.PlaySFX("RangeAttack");
    }

    public override void Update()
    {
        base.Update();
        // 스펠 공격 애니메이션 출력을 위한 딜레이
        m_player.RangeAttackDelay += Time.deltaTime;

        // 스펠 공격 애니메이션이 출력된 후
        // 마나를 1 소모 후 대기 상태로 전환
        if (m_player.RangeAttackDelay > 0.4f)
        {            
            m_player.RangeAttackDelay = 0;
            GameManager.Instance.UseMp(1);
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Idle]);
        }
    }
}

/// <summary>
/// 플레이어 죽음 상태
/// FixedUpdate : X
/// Transition X
/// </summary>
public class Player_Die : PlayerState
{
    public Player_Die(PlayerController _player) : base(_player)
    {
        HasPhysics = false;
    }

    public override void Enter()
    {
        m_player.Anim.Play(m_player.DIE_HASH);
        GameManager.Instance.GameOver();
    }
}