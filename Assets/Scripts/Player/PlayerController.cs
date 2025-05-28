using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float MoveSpeed;
    [SerializeField] public float JumpPow;
    [SerializeField] public GameObject m_spellPrefab;

    public StateMachine StateMach;
    public Animator Anim;
    public Rigidbody2D Rigid;
    public SpriteRenderer SpriteRenderer;
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
    public bool IsAim;

    public float MeleeAttackCoolTime;
    public float RangeAttackCoolTime;

    private void Awake()
    {
        Anim = GetComponent<Animator>();
        Rigid = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        JumpAction = GetComponent<PlayerInput>().actions["Jump"];
        MeleeAttackAction = GetComponent<PlayerInput>().actions["MeleeAttack"];
        RangeAttackAction = GetComponent<PlayerInput>().actions["RangedAttack"];
        Muzzle = GetComponentInChildren<PlayerMuzzle>();
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

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            IsJump = false;
            IsLand = true;
            Anim.SetBool("IsJump", IsJump);
        }
    }
}
