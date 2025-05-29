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

    public Transform TargetTransform;
    public bool IsMove;
    public Collider2D Player;
    public float AttackDelay;

    public Vector2 PatrolVec;

    private void Awake() => Init();

    private void Init()
    {
        Rigid = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();
        PatrolVec = Vector2.right;
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
        m_slimeData.MonsterHp -= damage;
        if(m_slimeData.MonsterHp < 0) m_slimeData.MonsterHp = 0;
    }
}
