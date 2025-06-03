using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ���� ���� ��ũ���ͺ� ������Ʈ�� ������ �޾�
/// ���� ������ �ൿ�� �ݿ��ϴ� ������Ʈ
/// ���� ���� ���� �ÿ� �ش� ������Ʈ�� ���̸� ��.
/// ���� �������� �� ������Ʈ�� ���峪, ���� ������ ���� �������� ���� ���� ��
/// </summary>
public class BossMonsterController : MonoBehaviour, IDamageable
{
    // ���� ���� ������(��ũ���ͺ� ������Ʈ)
    // ������ ��� ���� ��ũ��Ʈ�� ������Ʈ�� �������ּ���
    [SerializeField] public BossMonsterData BossMobData;
    // ���� ��� ���̾� - �÷��̾�� �������ּ���
    [SerializeField] public LayerMask TargetLayer;
    // �߰� ���ɿ��� ���� ���̾� - Ground�� �������ּ���
    [SerializeField] public LayerMask GroundLayer;
    // ���Ͱ� �׾��� �� ����ϴ� ���� - �ʿ信 ���� �������ּ���
    [SerializeField] private GameObject[] m_potions;
    // ���� �������� ����(�뷱��)
    [SerializeField] private float m_monsterAttackRange;
    // ���� ���� ���� ��Ÿ��
    [SerializeField] private float m_monsterSpellCooltime;

    [SerializeField] private float m_stunCoolTime;
    
    // ������Ʈ ����
    public StateMachine StateMach;
    public Rigidbody2D Rigid;
    public SpriteRenderer SpriteRenderer;
    public Animator Anim;
    public SFXController SFXCtrl;
    public BossMonsterMuzzle Muzzle;

    // ���� �ִϸ��̼�
    public readonly int IDLE_HASH = Animator.StringToHash("BossMonster_Idle");
    public readonly int MELEEATTACK_HASH = Animator.StringToHash("BossMonster_Attack");
    public readonly int DIE_HASH = Animator.StringToHash("BosslMonster_Die");

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
    private float m_bossMonsterHp;


    private void Awake() => Init();

    private void Init()
    {
        Rigid = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();
        Muzzle = GetComponentInChildren<BossMonsterMuzzle>();
        TryGetComponent<SFXController>(out SFXCtrl);
        PatrolVec = Vector2.right;
        m_bossMonsterHp = BossMobData.MonsterHp;
        m_monsterSpellCooltime = 10;
        StateMachineInit();
    }
    private void StateMachineInit()
    {
        StateMach = new StateMachine();
        StateMach.StateDic.Add(EState.Idle, new BossMonsterState_Idle(this));
        StateMach.StateDic.Add(EState.Trace, new BossMonsterState_Trace(this));
        StateMach.StateDic.Add(EState.MeleeAttack, new BossMonsterState_MeleeAttack(this));
        StateMach.StateDic.Add(EState.Stun, new BossMonsterState_Stun(this));
        StateMach.StateDic.Add(EState.Die, new BossMonsterState_Die(this));

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
        m_monsterSpellCooltime += Time.deltaTime;
        m_stunCoolTime += Time.deltaTime;
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
        m_bossMonsterHp -= damage;
        SFXCtrl.PlaySFX("Damage");
        if(m_stunCoolTime > 3)
        {
            m_stunCoolTime = 0;
            StateMach.ChangeState(StateMach.StateDic[EState.Stun]);
        }
        if (m_bossMonsterHp <= 0)
        {
            m_bossMonsterHp = 0;
            GameManager.Instance.ScorePlus(BossMobData.Score);
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
        Player = Physics2D.OverlapCircle(transform.position + Vector3.down, m_monsterAttackRange, TargetLayer);
        if (Player != null)
        {
            Player.GetComponent<IDamageable>().TakeDamage(BossMobData.MonsterAtk);
        }
    }

    /// <summary>
    /// ������ �ٴ� Ÿ�� �� �Ҹ� �߻�
    /// Animation Event �Լ��� �����
    /// ������ ���ݰ� ���ÿ� ������ ������ ��� �߰�
    /// ������ ��Ÿ���� ������ 10�ʸ��� ������ ������ ����
    /// </summary>
    public void MonsterSpell()
    {
        SFXCtrl.PlaySFX("Attack");
        if (m_monsterSpellCooltime > 10)
        {
            m_monsterSpellCooltime = 0;
            Muzzle.Fire();
        }
    }

    /// <summary>
    /// ������ ������ Ȯ���� ����ϴ� �ý���
    /// �ý��ۻ� ü�� ���ǰ� ���� ���� �� �� ������ ����ϸ�(�� ���� ���õ�� �Ұ�)
    /// Ȯ���� 11% (1/9) �� ������(�뷱��)
    /// </summary>
    private void DropPotion()
    {
        if (IsDead)
        {
            int rand = Random.Range(0, 10);
            if (rand == 7)
            {
                Instantiate(m_potions[0], transform.position, Quaternion.identity);
            }
            if (rand == 8)
            {
                Instantiate(m_potions[1], transform.position, Quaternion.identity);
            }
        }
    }
    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position, BossMobData.MonsterSight);
        Gizmos.DrawWireSphere(transform.position + Vector3.down, m_monsterAttackRange);
    }
}
