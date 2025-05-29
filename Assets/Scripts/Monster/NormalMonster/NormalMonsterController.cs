using UnityEngine;


public class NormalMonsterController : MonoBehaviour,IDamageable
{
    [SerializeField] public NormalMonsterData m_slimeData;

    public StateMachine StateMach;
    public Rigidbody2D Rigid;
    public SpriteRenderer SpriteRenderer;
    public Animator Anim;

    public readonly int IDLE_HASH = Animator.StringToHash("Slime_Idle");

    public bool IsMove;

    private void Awake() => Init();

    private void Init()
    {
        Rigid = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();
        StateMachineInit();
    }

    private void StateMachineInit()
    {
        StateMach = new StateMachine();
        StateMach.StateDic.Add(EState.Idle, new NormalMonsterState_Idle(this));
        StateMach.StateDic.Add(EState.Walk, new NormalMonsterState_Walk(this));
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
