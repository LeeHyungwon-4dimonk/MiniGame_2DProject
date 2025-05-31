using System.Collections;
using DesignPattern;
using UnityEngine;


public class NormalMonsterController : PooledObject, IDamageable
{
    [SerializeField] public NormalMonsterData m_slimeData;
    [SerializeField] public LayerMask TargetLayer;
    [SerializeField] public LayerMask GroundLayer;

    public StateMachine StateMach;
    public Rigidbody2D Rigid;
    public SpriteRenderer SpriteRenderer;
    public Animator Anim;
    public SFXController SFXCtrl;

    public readonly int IDLE_HASH = Animator.StringToHash("Slime_Idle");
    public readonly int MELEEATTACK_HASH = Animator.StringToHash("Slime_Attack");
    public readonly int DIE_HASH = Animator.StringToHash("Slime_Die");

    public Transform TargetTransform;
    public bool IsMove;
    public Collider2D Player;
    public float AttackDelay;

    private Coroutine m_coroutine;

    private float m_monsterHp;

    public Vector2 PatrolVec;

    private void Awake() => Init();

    private void Init()
    {
        Rigid = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();
        TryGetComponent<SFXController>(out SFXCtrl);
        PatrolVec = Vector2.right;
        m_monsterHp = m_slimeData.MonsterHp;
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
        if (!GameManager.Instance.IsGameOver())
        {
            StateMach.Update();
        }
        else
        {
            StateMach.ChangeState(StateMach.StateDic[EState.Idle]);
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.IsGameOver())
        {
            StateMach.FixedUpdate();
        }
    }

    public void TakeDamage(int damage)
    {
        SFXCtrl.PlaySFX("Damage");
        m_monsterHp -= damage;
        if (m_monsterHp <= 0)
        {
            StateMach.ChangeState(StateMach.StateDic[EState.Idle]);
            m_monsterHp = 0;
            GameManager.Instance.ScorePlus(100);
            if (m_coroutine != null)
            {
                StopCoroutine(m_coroutine);
                m_coroutine = null;
            }
            if (m_coroutine == null)
            {
                m_coroutine = StartCoroutine(Die());
            }
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
    IEnumerator Die()
    {
        yield return new WaitForSeconds(0.4f);
        ReturnPool();
    }
}
