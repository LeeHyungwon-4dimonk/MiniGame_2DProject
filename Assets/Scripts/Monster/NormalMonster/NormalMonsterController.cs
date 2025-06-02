using System.Collections;
using DesignPattern;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;


public class NormalMonsterController : PooledObject, IDamageable
{
    [SerializeField] public NormalMonsterData m_slimeData;
    [SerializeField] public LayerMask TargetLayer;
    [SerializeField] public LayerMask GroundLayer;
    [SerializeField] private GameObject[] m_potions;

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
    public bool IsStun;
    public bool IsDead;
    public Collider2D Player;
    public float AttackDelay;
    public float StunDelay;
    public float DieDelay;

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
        StateMach.StateDic.Add(EState.Stun, new NormalMonsterState_Stun(this));
        StateMach.StateDic.Add(EState.Die, new NormalMonsterState_Die(this));

        StateMach.CurState = StateMach.StateDic[EState.Idle];
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameOver() && !GameManager.Instance.IsGameClear())
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
        if (!GameManager.Instance.IsGameOver() && !GameManager.Instance.IsGameClear())
        {
            StateMach.FixedUpdate();
        }
    }

    private void OnDisable()
    {
        DropPotion();
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{damage}");
        m_monsterHp -= damage;
        SFXCtrl.PlaySFX("Damage");
        StateMach.ChangeState(StateMach.StateDic[EState.Stun]);
        if (m_monsterHp <= 0)
        {
            m_monsterHp = 0;
            GameManager.Instance.ScorePlus(100);
            StateMach.ChangeState(StateMach.StateDic[EState.Die]);
        }
    }

    public void Attack()
    {
        Player = Physics2D.OverlapCircle(transform.position + Vector3.up, 2f, TargetLayer);
        if (Player != null)
        {
            Player.GetComponent<IDamageable>().TakeDamage(m_slimeData.MonsterAtk);
        }
    }

    private void DropPotion()
    {
        if(IsDead)
        {
            int rand = Random.Range(0, 10);
            if(rand == 7)
            {
                Instantiate(m_potions[0], transform.position, Quaternion.identity);
            }
            if(rand == 8)
            {
                Instantiate(m_potions[1], transform.position, Quaternion.identity);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_slimeData.MonsterSight);
        Gizmos.DrawWireSphere(transform.position+Vector3.up, 1.5f);
    }
}
