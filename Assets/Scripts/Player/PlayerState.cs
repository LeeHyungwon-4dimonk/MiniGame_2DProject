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
    /// �ǽð� ������Ʈ ���� ����
    /// * �ٰŸ� ����
    /// * ����
    /// * ����
    /// </summary>
    public override void Update()
    {
        // �÷��̾ ��¡ ���³� ���� ���� �߻� ���� �ƴ� ��
        // �÷��̾��� ���� ��Ÿ���� ����
        if(m_player.StateMach.CurState != m_player.StateMach.StateDic[EState.Charge]
            && m_player.StateMach.CurState != m_player.StateMach.StateDic[EState.RangedAttack])
        {
            GameManager.Instance.SetCoolTime();
        }

        // �÷��̾ ��¡ ���³� ���� ���� �߻����� �ƴ� ��
        // �÷��̾��� ���콺�� �Ͻ����� ��ư�� �ö� �ִ� ���� �ƴ� ��
        // �÷��̾�� ���콺 Ŭ���� ���� �� ���� ������ �� �� ����
        // (��� ĵ�� ������)
        if (!m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_ChargeSpell")
            && !m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_SpellAttack")
            && m_player.MeleeAttackAction.IsPressed() && !GameManager.Instance.IsTryPause())
        {
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.MeleeAttack]);
        }

        // �÷��̾ ���� ���� ���� �ƴ� ��
        // �÷��̾ ������ �ְ� ��Ÿ���� �� �Ǿ��� ��
        // �÷��̾�� ���콺 ��Ŭ���� ���� �� ���� ������ �� �� ����
        if(!m_player.Anim.GetCurrentAnimatorStateInfo(0).IsName("Player_MeleeAttack")
            && m_player.RangeAttackAction.IsPressed() && GameManager.Instance.GetCoolTime() < 0
            && GameManager.Instance.GetCurMP() > 0)
        {
            GameManager.Instance.SetCoolTime(3);
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Charge]);
        }

        // �÷��̾ ���� ���� ���� �ƴ� ��
        // �÷��̾ ��¡ ���³� ���� ���� �߻����� �ƴ� ��
        // �÷��̾ ���� ������ ������ ��
        // �÷��̾�� �����̽��ٸ� ������ �� ������ �� �� ����
        // (��� ĵ�� ������)
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

    // �ߺ� �ڵ带 �θ� ������Ʈ�� �Űܼ� ��� ( Walk, Jump���� ��� )
    // �÷��̾��� �Է¿� ���� �ִϸ��̼��� Flip, �浹ü�� Flip�ϴ� ���
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
/// �÷��̾� ��� ����
/// FixedUpdate : X
/// Transition -> �ȱ�
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

        // Input �Է°��� ���� �� �ȱ� ���·� ��ȯ
        if(Mathf.Abs(m_player.InputX.x) > 0.1f)
        {
            m_player.IsMove = true;
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Walk]);            
        }       
    }
}

/// <summary>
/// �÷��̾� �ȱ� ����
/// FixedUpdate : O
/// Transition -> ���
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

        // Input �Է°��� ���� �� ��� ���·� ��ȯ
        if(Mathf.Abs(m_player.InputX.x) < 0.1f)
        {
            m_player.IsMove = false;
            m_player.SFXCtrl.StopSFX();
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Idle]);            
        }

        // Input �Է°��� ���� �� �÷��̾��� �ִϸ��̼� ��� ���� �� �浹ü ���� ��ȯ
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
/// �÷��̾� ���� ����
/// FixedUpdate : O
/// Transition -> ���
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
        
        // �÷��̾ �ٴ����� �������� �� ��� ���·� ��ȯ
        if(m_player.IsLand)
        {
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Idle]);
        }

        // Input �Է°��� ���� �� �÷��̾��� �ִϸ��̼� ��� ���� �� �浹ü ���� ��ȯ 
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
/// �÷��̾� ���� ���� ����
/// FixedUpdate : X
/// Transition : ���
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
        // ���� �ִϸ��̼� ��� �ð��� �ݿ��� ������ �ð�
        m_player.MeleeAttackDelay += Time.deltaTime;

        // ���� �ִϸ��̼� ��� �� ��� ���·� ��ȯ
        if (m_player.MeleeAttackDelay > 0.8)
        {
            m_player.MeleeAttackDelay = 0;
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Idle]);            
        }
    }
}

/// <summary>
/// �÷��̾� ���� ����
/// FixedUpdate : X
/// Transition -> ���� ����
/// �� �ܰ迡�� ������ ĵ���� ����� �������� ����(TODO)
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
        // �÷��̾��� ���� �ð��� ���� ������ �ݿ��� ���� ��ġ �ݿ�
        m_player.ChargeTime += Time.deltaTime;

        // �÷��̾ ���콺Ŭ�� ���¿��� Ű�� ���� �� ���� ���� ���·� ��ȯ
        if (m_player.RangeAttackAction.WasReleasedThisFrame())
        {
            m_player.SFXCtrl.StopSFX();
            m_player.IsCharging = false;
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.RangedAttack]);
        }
    }
}

/// <summary>
/// �÷��̾� ���� ����
/// FixedUpdate: X
/// Transition -> ���
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
        // ���� ���� �ִϸ��̼� ����� ���� ������
        m_player.RangeAttackDelay += Time.deltaTime;

        // ���� ���� �ִϸ��̼��� ��µ� ��
        // ������ 1 �Ҹ� �� ��� ���·� ��ȯ
        if (m_player.RangeAttackDelay > 0.4f)
        {            
            m_player.RangeAttackDelay = 0;
            GameManager.Instance.UseMp(1);
            m_player.StateMach.ChangeState(m_player.StateMach.StateDic[EState.Idle]);
        }
    }
}

/// <summary>
/// �÷��̾� ���� ����
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