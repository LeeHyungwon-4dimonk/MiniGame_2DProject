using DesignPattern;
using UnityEngine;

/// <summary>
/// 노멀 몬스터 스크립터블 오브젝트로 정보를 받아
/// 노멀 몬스터의 행동을 반영하는 컴포넌트
/// 노멀 몬스터 생성 시에 해당 컴포넌트를 붙이면 됨.
/// 또한 오브젝트 풀 패턴으로 구현되어
/// 몬스터를 한 번에 여러 마리를 소환하거나 리젠에 사용할 수 있음
/// 몬스터 프리팹은 이 컴포넌트로 만드나, 몬스터 생성은 생성 프리팹을 따로 만들 것
/// * 몬스터 수치 밸런싱이 필요할 경우 스크립터블 오브젝트에서 내용을 수정할 것
/// </summary>
public class NormalMonsterController : PooledObject, IDamageable
{
    // 노멀 몬스터 데이터(스크립터블 오브젝트)
    // 생성할 노멀 몬스터 스크립트블 오브젝트를 참조해주세요
    [SerializeField] public NormalMonsterData NormalMobData;
    // 공격 대상 레이어 - 플레이어로 설정해주세요
    [SerializeField] public LayerMask TargetLayer;
    // 추격 가능여부 판정 레이어 - Ground로 설정해주세요
    [SerializeField] public LayerMask GroundLayer;
    // 몬스터가 죽었을 때 드랍하는 포션 - 필요에 따라 설정해주세요
    [SerializeField] private GameObject[] m_potions;

    // 컴포넌트 참조
    public StateMachine StateMach;
    public Rigidbody2D Rigid;
    public SpriteRenderer SpriteRenderer;
    public Animator Anim;
    public SFXController SFXCtrl;

    // 몬스터 애니메이션
    public readonly int IDLE_HASH = Animator.StringToHash("NormalMonster_Idle");
    public readonly int MELEEATTACK_HASH = Animator.StringToHash("NormalMonster_Attack");
    public readonly int DIE_HASH = Animator.StringToHash("NormalMonster_Die");

    // 몬스터 상태 변수
    public bool IsMove;
    public bool IsStun;
    public bool IsDead;

    // 몬스터 플레이어 판정용 변수
    public Collider2D Player;
    public Transform TargetTransform;

    // 몬스터 시간 변수
    public float AttackDelay;
    public float StunDelay;
    public float DieDelay;

    // 몬스터 탐지 방향
    public Vector2 PatrolVec;
    
    // 몬스터 체력
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

    // 상태머신 상태 등록
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
        // 게임이 끝난 상태가 아니면
        if (!GameManager.Instance.IsGameOver() && !GameManager.Instance.IsGameClear())
        {
            StateMach.Update();
        }
        // 게임 종료 후에도 몬스터가 행동하는 것 방지
        else
        {
            StateMach.ChangeState(StateMach.StateDic[EState.Idle]);
        }        
    }

    private void FixedUpdate()
    {
        // 게임이 끝난 상태가 아니면
        if (!GameManager.Instance.IsGameOver() && !GameManager.Instance.IsGameClear())
        {
            StateMach.FixedUpdate();
        }
    }

    // 몬스터가 비활성화 되었을 때(죽었을 때)
    // 포션을 드랍함
    private void OnDisable()
    {
        DropPotion();
    }

    /// <summary>
    /// 몬스터 데미지 (damage 수치만큼)
    /// 피격 시에 효과음이 재생됨
    /// 현재 피격음이 없을 경우에 대한 예외처리가 되어 있지 않으므로
    /// 필수적으로 피격음을 넣을 것
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
    /// 몬스터의 공격 반영
    /// Animation Event 함수로 사용함
    /// 몬스터의 근접 공격 범위 판정 밸런싱이 필요할 시 여기에서 수정
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
    /// 포션은 랜덤한 확률로 드랍하는 시스템
    /// 시스템상 체력 포션과 마나 포션 중 한 종류만 드랍하며(두 가지 동시드랍 불가)
    /// 확률은 11% (1/9) 로 설정함(밸런싱)
    ///  => 테스트 진행 결과 실제 드랍 수치가 너무 낮은 것 같아 22%로 변경 (2/9) // #1 밸런스 패치
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