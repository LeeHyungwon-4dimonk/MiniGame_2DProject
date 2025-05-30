using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] public float MoveSpeed;
    [SerializeField] public float JumpPow;

    [SerializeField] private int m_playerMeleeAttack;
    [SerializeField] private LayerMask m_layerMask;

    public StateMachine StateMach;
    public Animator Anim;
    public Rigidbody2D Rigid;
    public SpriteRenderer SpriteRenderer;
    public CapsuleCollider2D PlayerCollider;
    public PlayerMuzzle Muzzle;

    public Vector2 InputX;
    public InputAction JumpAction;
    public InputAction MeleeAttackAction;
    public InputAction RangeAttackAction;

    public readonly int IDLE_HASH = Animator.StringToHash("Player_Idle");
    public readonly int MELEEATTACK_HASH = Animator.StringToHash("Player_MeleeAttack");
    public readonly int CHARGE_HASH = Animator.StringToHash("Player_ChargeSpell");
    public readonly int SPELLATTACK_HASH = Animator.StringToHash("Player_SpellAttack");

    public bool IsMove;
    public bool IsJump;
    public bool IsLand;

    public float MeleeAttackDelay;
    public float RangeAttackDelay;
    public float RangeAttackCoolTime = 3;

    public int PlayerHp;

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
        PlayerHp = 10;
        StateMachineInit();
    }

    private void StateMachineInit()
    {
        StateMach = new StateMachine();
        StateMach.StateDic.Add(EState.Idle, new Player_Idle(this));
        StateMach.StateDic.Add(EState.Walk, new Player_Walk(this));
        StateMach.StateDic.Add(EState.Jump, new Player_Jump(this));
        StateMach.StateDic.Add(EState.MeleeAttack, new Player_MeleeAttack(this));
        StateMach.StateDic.Add(EState.Charge, new Player_Charge(this));
        StateMach.StateDic.Add(EState.RangedAttack, new Player_RangeAttack(this));

        StateMach.CurState = StateMach.StateDic[EState.Idle];
    }

    private void Update()
    {
        StateMach.Update();
    }

    private void FixedUpdate()
    {
        StateMach.FixedUpdate();
    }

    public void OnMove(InputValue value)
    {
        InputX = value.Get<Vector2>();
        InputX.Normalize();
    }

    public void TakeDamage(int damage)
    {
        PlayerHp -= damage;
        if (PlayerHp < 0)
        { 
            PlayerHp = 0;
            gameObject.SetActive(false);
        }
    }

    public void Attack()
    {
        Collider2D Monster;
        Monster = Physics2D.OverlapCircle(transform.position, 1f, m_layerMask);
        if (Monster != null)
        {
            Monster.GetComponent<IDamageable>().TakeDamage(m_playerMeleeAttack);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        IsJump = false;
        IsLand = true;
        Anim.SetBool("IsJump", IsJump);
    }
}
