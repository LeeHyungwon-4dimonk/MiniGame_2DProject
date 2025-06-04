using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어에 직접 붙이는 컨트롤러 컴포넌트
/// </summary>
public class PlayerController : MonoBehaviour, IDamageable
{
    // 밸런스용 변수들
    [Header("Player Balance")]
    // 플레이어 이동 속도(밸런싱)
    [SerializeField] public float MoveSpeed;
    // 플레이어 점프력(밸런싱)
    [SerializeField] public float JumpPow;
    // 플레이어 공격력(밸런싱)
    [SerializeField] private int m_playerMeleeAttack;
    // 플레이어 근접공격 범위(밸런싱)
    [SerializeField] private float m_playerAttackRange;
    // 플레이어 스펠 공격 쿨타임(밸런싱)
    [SerializeField] public float SpellCoolTime;
    // 플레이어 스펠 마나 소모량(밸런싱)
    [SerializeField] public int ManaConsumption;

    [Header("Animation Delay")]
    // 플레이어 공격 애니메이션 출력 딜레이
    [SerializeField] public float MeleeAttackMotionDelay;
    // 플레이어 스펠 공격 애니메이션 출력 딜레이
    [SerializeField] public float RangedAttackMotionDelay;

    [Header("Attack Target")]
    // 플레이어 타겟
    [SerializeField] private LayerMask m_layerMask;

    // 플레이어 상태머신
    public StateMachine StateMach;

    // 컴포넌트 참조
    public Animator Anim;
    public Rigidbody2D Rigid;
    public SpriteRenderer SpriteRenderer;
    public CapsuleCollider2D PlayerCollider;
    public PlayerMuzzle Muzzle;
    public SFXController SFXCtrl;

    // 플레이어 컨트롤 변수
    public Vector2 InputX;
    public InputAction JumpAction;
    public InputAction MeleeAttackAction;
    public InputAction RangeAttackAction;

    // 플레이어 애니메이션
    public readonly int IDLE_HASH = Animator.StringToHash("Player_Idle");
    public readonly int MELEEATTACK_HASH = Animator.StringToHash("Player_MeleeAttack");
    public readonly int CHARGE_HASH = Animator.StringToHash("Player_ChargeSpell");
    public readonly int SPELLATTACK_HASH = Animator.StringToHash("Player_SpellAttack");
    public readonly int DIE_HASH = Animator.StringToHash("Player_Die");

    // 플레이어 상태 변수(bool)
    public bool IsMove;
    public bool IsJump;
    public bool IsLand;
    public bool IsCharging;

    // 플레이어 시간 변수
    public float MeleeAttackDelay;
    public float RangeAttackDelay;
    public float ChargeTime;

    // 플레이어 몬스터 거리 판정용
    private Collider2D m_attackable;

    private void Awake() => Init();

    private void Init()
    {
        Anim = GetComponent<Animator>();
        Rigid = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        PlayerCollider = GetComponent<CapsuleCollider2D>();
        JumpAction = GetComponent<PlayerInput>().actions["Jump"];
        MeleeAttackAction = GetComponent<PlayerInput>().actions["MeleeAttack"];
        RangeAttackAction = GetComponent<PlayerInput>().actions["RangedAttack"];
        Muzzle = GetComponentInChildren<PlayerMuzzle>();
        SFXCtrl = GetComponent<SFXController>();

        StateMachineInit();
    }

    // 상태머신 상태 등록
    private void StateMachineInit()
    {
        StateMach = new StateMachine();
        StateMach.StateDic.Add(EState.Idle, new Player_Idle(this));
        StateMach.StateDic.Add(EState.Walk, new Player_Walk(this));
        StateMach.StateDic.Add(EState.Jump, new Player_Jump(this));
        StateMach.StateDic.Add(EState.MeleeAttack, new Player_MeleeAttack(this));
        StateMach.StateDic.Add(EState.Charge, new Player_Charge(this));
        StateMach.StateDic.Add(EState.RangedAttack, new Player_RangeAttack(this));
        StateMach.StateDic.Add(EState.Die, new Player_Die(this));

        StateMach.CurState = StateMach.StateDic[EState.Idle];
    }

    private void Update()
    {
        // 게임이 끝난 상태가 아니면
        if (!GameManager.Instance.IsGameOver() && !GameManager.Instance.IsGameClear())
        {
            StateMach.Update();
        }

        // 게임 종료 후에도 반복 재생되는 효과음 재생 방지
        else
        {
            SFXCtrl.StopSFX();
        }
    }

    private void FixedUpdate()
    {
        // 게임이 끝난 상태가 아니면
        if (!GameManager.Instance.IsGameOver() && !GameManager.Instance.IsGameClear())
        {
            StateMach.FixedUpdate();
        }
    }

    // InputSystem 입력
    public void OnMove(InputValue value)
    {
        InputX = value.Get<Vector2>();
        InputX.Normalize();
    }

    /// <summary>
    /// 플레이어 데미지 (damamge 수치만큼)
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        GameManager.Instance.DamageHp(damage);
        //Debug.Log("아야");
        if (GameManager.Instance.GetCurHP() == 0)
        {
            StateMach.ChangeState(StateMach.StateDic[EState.Die]);
        }
    }

    /// <summary>
    /// 플레이어의 공격 반영
    /// Animation Event 함수로 사용함
    /// 플레이어의 근접 공격 범위 판정 밸런싱이 필요할 시 여기에서 수정
    /// </summary>
    public void Attack()
    {
        m_attackable = Physics2D.OverlapCircle(transform.position + Vector3.up, m_playerAttackRange, m_layerMask);

        if (m_attackable != null)
        {
            m_attackable.GetComponent<IDamageable>().TakeDamage(m_playerMeleeAttack);
        }
    }

    // 플레이어 바닥 접촉 시 점프 가능
    public void OnCollisionEnter2D(Collision2D collision)
    {
        IsJump = false;
        IsLand = true;
        Anim.SetBool("IsJump", IsJump);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + Vector3.up, m_playerAttackRange);
    }
}