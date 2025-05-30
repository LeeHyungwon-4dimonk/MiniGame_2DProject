using UnityEngine;


public class NormalMonsterController : MonoBehaviour,IDamageable
{
    [SerializeField] public NormalMonsterData m_slimeData;
    [SerializeField] public LayerMask TargetLayer;
    [SerializeField] public LayerMask GroundLayer;

    public StateMachine StateMach;
    public Rigidbody2D Rigid;
    public SpriteRenderer SpriteRenderer;
    public Animator Anim;

    public readonly int IDLE_HASH = Animator.StringToHash("Slime_Idle");
    public readonly int MELEEATTACK_HASH = Animator.StringToHash("Slime_Attack");
    public readonly int DIE_HASH = Animator.StringToHash("Slime_Die");

    public Transform TargetTransform;
    public bool IsMove;
    public Collider2D Player;
    public float AttackDelay;

    private float m_MonsterHp;

    public Vector2 PatrolVec;

    private void Awake() => Init();

    private void Init()
    {
        Rigid = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();
        PatrolVec = Vector2.right;
        m_MonsterHp = m_slimeData.MonsterHp;
        StateMachineInit();
    }

    private void StateMachineInit()
    {
        StateMach = new StateMachine();
        StateMach.StateDic.Add(EState.Idle, new NormalMonsterState_Idle(this));
        StateMach.StateDic.Add(EState.Walk, new NormalMonsterState_Walk(this));
        StateMach.StateDic.Add(EState.Trace, new NormalMonsterState_Trace(this));
        StateMach.StateDic.Add(EState.MeleeAttack, new NormalMonsterState_MeleeAttack(this));

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

    public void TakeDamage(int damage)
    {
        m_MonsterHp-= damage;
        if (m_MonsterHp < 0)
        {
            m_MonsterHp = 0;
            gameObject.SetActive(false);
        }
    }

    public void Attack()
    {
        Player = Physics2D.OverlapCircle(transform.position, 1f, TargetLayer);
        if (Player != null)
        {
            Player.GetComponent<IDamageable>().TakeDamage(m_slimeData.MonsterAtk);
        }
    }
}
