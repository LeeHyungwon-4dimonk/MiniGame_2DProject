using DesignPattern;
using UnityEngine;

/// <summary>
/// ��� ���� ��ũ���ͺ� ������Ʈ�� ������ �޾�
/// ��� ������ �ൿ�� �ݿ��ϴ� ������Ʈ
/// ��� ���� ���� �ÿ� �ش� ������Ʈ�� ���̸� ��.
/// ���� ������Ʈ Ǯ �������� �����Ǿ�
/// ���͸� �� ���� ���� ������ ��ȯ�ϰų� ������ ����� �� ����
/// ���� �������� �� ������Ʈ�� ���峪, ���� ������ ���� �������� ���� ���� ��
/// * ���� ��ġ �뷱���� �ʿ��� ��� ��ũ���ͺ� ������Ʈ���� ������ ������ ��
/// </summary>
public class NormalMonsterController : PooledObject, IDamageable
{
    // ��� ���� ������(��ũ���ͺ� ������Ʈ)
    // ������ ��� ���� ��ũ��Ʈ�� ������Ʈ�� �������ּ���
    [SerializeField] public NormalMonsterData NormalMobData;
    // ���� ��� ���̾� - �÷��̾�� �������ּ���
    [SerializeField] public LayerMask TargetLayer;
    // �߰� ���ɿ��� ���� ���̾� - Ground�� �������ּ���
    [SerializeField] public LayerMask GroundLayer;
    // ���Ͱ� �׾��� �� ����ϴ� ���� - �ʿ信 ���� �������ּ���
    [SerializeField] private GameObject[] m_potions;

    // ������Ʈ ����
    public StateMachine StateMach;
    public Rigidbody2D Rigid;
    public SpriteRenderer SpriteRenderer;
    public Animator Anim;
    public SFXController SFXCtrl;

    // ���� �ִϸ��̼�
    public readonly int IDLE_HASH = Animator.StringToHash("NormalMonster_Idle");
    public readonly int MELEEATTACK_HASH = Animator.StringToHash("NormalMonster_Attack");
    public readonly int DIE_HASH = Animator.StringToHash("NormalMonster_Die");

    // ���� ���� ����
    public bool IsMove;
    public bool IsStun;
    public bool IsDead;

    // ���� �÷��̾� ������ ����
    public Collider2D Player;
    public Transform TargetTransform;

    // ���� �ð� ����
    public float AttackDelay;
    public float StunDelay;
    public float DieDelay;

    // ���� Ž�� ����
    public Vector2 PatrolVec;
    
    // ���� ü��
    private float m_normalMonsterHp;

    private void Awake() => Init();

    private void Init()
    {
        Rigid = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();
        TryGetComponent<SFXController>(out SFXCtrl);
        PatrolVec = Vector2.right;
        m_normalMonsterHp = NormalMobData.MonsterHp;
        StateMachineInit();
    }

    // ���¸ӽ� ���� ���
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
        // ������ ���� ���°� �ƴϸ�
        if (!GameManager.Instance.IsGameOver() && !GameManager.Instance.IsGameClear())
        {
            StateMach.Update();
        }
        // ���� ���� �Ŀ��� ���Ͱ� �ൿ�ϴ� �� ����
        else
        {
            StateMach.ChangeState(StateMach.StateDic[EState.Idle]);
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

    // ���Ͱ� ��Ȱ��ȭ �Ǿ��� ��(�׾��� ��)
    // ������ �����
    private void OnDisable()
    {
        DropPotion();
    }

    /// <summary>
    /// ���� ������ (damage ��ġ��ŭ)
    /// �ǰ� �ÿ� ȿ������ �����
    /// ���� �ǰ����� ���� ��쿡 ���� ����ó���� �Ǿ� ���� �����Ƿ�
    /// �ʼ������� �ǰ����� ���� ��
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        //Debug.Log($"{damage}");
        m_normalMonsterHp -= damage;
        SFXCtrl.PlaySFX("Damage");
        StateMach.ChangeState(StateMach.StateDic[EState.Stun]);
        if (m_normalMonsterHp <= 0)
        {
            m_normalMonsterHp = 0;
            GameManager.Instance.ScorePlus(NormalMobData.Score);
            StateMach.ChangeState(StateMach.StateDic[EState.Die]);
        }
    }

    /// <summary>
    /// ������ ���� �ݿ�
    /// Animation Event �Լ��� �����
    /// ������ ���� ���� ���� ���� �뷱���� �ʿ��� �� ���⿡�� ����
    /// </summary>
    public void Attack()
    {
        Player = Physics2D.OverlapCircle(transform.position + Vector3.up, NormalMobData.AttackRange, TargetLayer);
        if (Player != null)
        {
            Player.GetComponent<IDamageable>().TakeDamage(NormalMobData.MonsterAtk);
        }
    }

    /// <summary>
    /// ������ ������ Ȯ���� ����ϴ� �ý���
    /// �ý��ۻ� ü�� ���ǰ� ���� ���� �� �� ������ ����ϸ�(�� ���� ���õ�� �Ұ�)
    /// Ȯ���� 11% (1/9) �� ������(�뷱��)
    ///  => �׽�Ʈ ���� ��� ���� ��� ��ġ�� �ʹ� ���� �� ���� 22%�� ���� (2/9) // #1 �뷱�� ��ġ
    /// </summary>
    private void DropPotion()
    {
        if(IsDead)
        {
            int rand = Random.Range(0, 10);
            if (rand == 6 || rand == 7)
            {
                Instantiate(m_potions[0], transform.position, Quaternion.identity);
            }
            if (rand == 8 || rand == 9)
            {
                Instantiate(m_potions[1], transform.position, Quaternion.identity);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, NormalMobData.MonsterSight);
        Gizmos.DrawWireSphere(transform.position + Vector3.up, NormalMobData.AttackRange);
    }
}