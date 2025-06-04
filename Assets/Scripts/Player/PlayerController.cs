using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾ ���� ���̴� ��Ʈ�ѷ� ������Ʈ
/// </summary>
public class PlayerController : MonoBehaviour, IDamageable
{
    // �뷱���� ������
    [Header("Player Balance")]
    // �÷��̾� �̵� �ӵ�(�뷱��)
    [SerializeField] public float MoveSpeed;
    // �÷��̾� ������(�뷱��)
    [SerializeField] public float JumpPow;
    // �÷��̾� ���ݷ�(�뷱��)
    [SerializeField] private int m_playerMeleeAttack;
    // �÷��̾� �������� ����(�뷱��)
    [SerializeField] private float m_playerAttackRange;
    // �÷��̾� ���� ���� ��Ÿ��(�뷱��)
    [SerializeField] public float SpellCoolTime;
    // �÷��̾� ���� ���� �Ҹ�(�뷱��)
    [SerializeField] public int ManaConsumption;

    [Header("Animation Delay")]
    // �÷��̾� ���� �ִϸ��̼� ��� ������
    [SerializeField] public float MeleeAttackMotionDelay;
    // �÷��̾� ���� ���� �ִϸ��̼� ��� ������
    [SerializeField] public float RangedAttackMotionDelay;

    [Header("Attack Target")]
    // �÷��̾� Ÿ��
    [SerializeField] private LayerMask m_layerMask;

    // �÷��̾� ���¸ӽ�
    public StateMachine StateMach;

    // ������Ʈ ����
    public Animator Anim;
    public Rigidbody2D Rigid;
    public SpriteRenderer SpriteRenderer;
    public CapsuleCollider2D PlayerCollider;
    public PlayerMuzzle Muzzle;
    public SFXController SFXCtrl;

    // �÷��̾� ��Ʈ�� ����
    public Vector2 InputX;
    public InputAction JumpAction;
    public InputAction MeleeAttackAction;
    public InputAction RangeAttackAction;

    // �÷��̾� �ִϸ��̼�
    public readonly int IDLE_HASH = Animator.StringToHash("Player_Idle");
    public readonly int MELEEATTACK_HASH = Animator.StringToHash("Player_MeleeAttack");
    public readonly int CHARGE_HASH = Animator.StringToHash("Player_ChargeSpell");
    public readonly int SPELLATTACK_HASH = Animator.StringToHash("Player_SpellAttack");
    public readonly int DIE_HASH = Animator.StringToHash("Player_Die");

    // �÷��̾� ���� ����(bool)
    public bool IsMove;
    public bool IsJump;
    public bool IsLand;
    public bool IsCharging;

    // �÷��̾� �ð� ����
    public float MeleeAttackDelay;
    public float RangeAttackDelay;
    public float ChargeTime;

    // �÷��̾� ���� �Ÿ� ������
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

    // ���¸ӽ� ���� ���
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
        // ������ ���� ���°� �ƴϸ�
        if (!GameManager.Instance.IsGameOver() && !GameManager.Instance.IsGameClear())
        {
            StateMach.Update();
        }

        // ���� ���� �Ŀ��� �ݺ� ����Ǵ� ȿ���� ��� ����
        else
        {
            SFXCtrl.StopSFX();
        }
    }

    private void FixedUpdate()
    {
        // ������ ���� ���°� �ƴϸ�
        if (!GameManager.Instance.IsGameOver() && !GameManager.Instance.IsGameClear())
        {
            StateMach.FixedUpdate();
        }
    }

    // InputSystem �Է�
    public void OnMove(InputValue value)
    {
        InputX = value.Get<Vector2>();
        InputX.Normalize();
    }

    /// <summary>
    /// �÷��̾� ������ (damamge ��ġ��ŭ)
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        GameManager.Instance.DamageHp(damage);
        //Debug.Log("�ƾ�");
        if (GameManager.Instance.GetCurHP() == 0)
        {
            StateMach.ChangeState(StateMach.StateDic[EState.Die]);
        }
    }

    /// <summary>
    /// �÷��̾��� ���� �ݿ�
    /// Animation Event �Լ��� �����
    /// �÷��̾��� ���� ���� ���� ���� �뷱���� �ʿ��� �� ���⿡�� ����
    /// </summary>
    public void Attack()
    {
        m_attackable = Physics2D.OverlapCircle(transform.position + Vector3.up, m_playerAttackRange, m_layerMask);

        if (m_attackable != null)
        {
            m_attackable.GetComponent<IDamageable>().TakeDamage(m_playerMeleeAttack);
        }
    }

    // �÷��̾� �ٴ� ���� �� ���� ����
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