using UnityEngine;

/// <summary>
/// 보스 몬스터 스크립터블 오브젝트로 정보를 받아
/// 보스 몬스터의 행동을 반영하는 컴포넌트
/// 보스 몬스터 생성 시에 해당 컴포넌트를 붙이면 됨.
/// * 몬스터 수치 밸런싱이 필요할 경우 스크립터블 오브젝트에서 내용을 수정할 것
/// </summary>
public class BossMonsterController : MonoBehaviour, IDamageable
{
    // 보스 몬스터 데이터(스크립터블 오브젝트)
    // 생성할 보스 몬스터 스크립트블 오브젝트를 참조해주세요
    [SerializeField] public BossMonsterData BossMobData;
    // 공격 대상 레이어 - 플레이어로 설정해주세요
    [SerializeField] public LayerMask TargetLayer;
    // 추격 가능여부 판정 레이어 - Ground로 설정해주세요
    [SerializeField] public LayerMask GroundLayer;
    // 보스 몬스터가 죽었을 때 드랍하는 포션 - 필요에 따라 설정해주세요
    [SerializeField] private GameObject[] m_potions;
    
    // 컴포넌트 참조
    public StateMachine StateMach;
    public Rigidbody2D Rigid;
    public SpriteRenderer SpriteRenderer;
    public Animator Anim;
    public SFXController SFXCtrl;
    public BossMonsterMuzzle Muzzle;

    // 보스 몬스터 애니메이션
    public readonly int IDLE_HASH = Animator.StringToHash("BossMonster_Idle");
    public readonly int MELEEATTACK_HASH = Animator.StringToHash("BossMonster_Attack");
    public readonly int DIE_HASH = Animator.StringToHash("BosslMonster_Die");

    // 보스 몬스터 상태 변수
    public bool IsMove;
    public bool IsStun;
    public bool IsDead;

    // 보스 몬스터 플레이어 판정용 변수
    public Collider2D Player;
    public Transform TargetTransform;

    // 보스 몬스터 시간 변수
    public float AttackDelay;
    public float StunDelay;
    public float DieDelay;

    // 보스 몬스터 탐지 방향
    public Vector2 PatrolVec;

    // 보스 몬스터 체력
    private float m_bossMonsterHp;
    // 보스 몬스터 스펠 공격 쿨타임
    private float m_monsterSpellCooltime;
    // 보스몬스터 스턴 쿨타임
    private float m_stunCoolTime;

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
        m_monsterSpellCooltime = BossMobData.SkillCoolTime;
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
        // 게임이 끝난 상태가 아니면
        if (!GameManager.Instance.IsGameOver() && !GameManager.Instance.IsGameClear())
        {
            StateMach.Update();
        }
        // 게임 종료 후에도 보스 몬스터가 행동하는 것 방지
        else
        {
            StateMach.ChangeState(StateMach.StateDic[EState.Idle]);
        }
        m_monsterSpellCooltime += Time.deltaTime;
        m_stunCoolTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // 게임이 끝난 상태가 아니면
        if (!GameManager.Instance.IsGameOver() && !GameManager.Instance.IsGameClear())
        {
            StateMach.FixedUpdate();
        }
    }

    // 보스 몬스터가 비활성화 되었을 때(죽었을 때)
    // 포션을 드랍함
    private void OnDisable()
    {
        DropPotion();
    }

    /// <summary>
    /// 보스 몬스터 데미지 (damage 수치만큼)
    /// 피격 시에 효과음이 재생됨
    /// 현재 피격음이 없을 경우에 대한 예외처리가 되어 있지 않으므로
    /// 필수적으로 피격음을 넣을 것
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        //Debug.Log($"{damage}");
        m_bossMonsterHp -= damage;
        SFXCtrl.PlaySFX("Damage");
        if(m_stunCoolTime > BossMobData.StunApplyCoolTime)
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
    /// 보스 몬스터의 공격 반영
    /// Animation Event 함수로 사용함
    /// 몬스터의 근접 공격 범위 판정 밸런싱이 필요할 시
    /// 스크립터블 오브젝트에서 수치 수정 / 범위 좌표 변경은 여기에서 수정
    /// </summary>
    public void Attack()
    {        
        Player = Physics2D.OverlapCircle(transform.position + Vector3.up, BossMobData.AttackRange, TargetLayer);
        if (Player != null)
        {
            Player.GetComponent<IDamageable>().TakeDamage(BossMobData.MonsterAtk);
        }
    }

    /// <summary>
    /// 몬스터의 바닥 타격 시 소리 발생
    /// Animation Event 함수로 사용함
    /// 몬스터의 공격과 동시에 스펠이 나가는 기능 추가
    /// 스펠은 쿨타임이 있으며 10초마다 나가는 것으로 설정
    /// (스펠 쿨타임은 BossMonsterData 스크립터블 오브젝트에서 수정)
    /// </summary>
    public void MonsterSpell()
    {
        SFXCtrl.PlaySFX("Attack");
        if (m_monsterSpellCooltime > BossMobData.SkillCoolTime)
        {
            m_monsterSpellCooltime = 0;
            Muzzle.Fire();
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
        if (IsDead)
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
        //Gizmos.DrawWireSphere(transform.position, BossMobData.MonsterSight);
        Gizmos.DrawWireSphere(transform.position + Vector3.up, BossMobData.AttackRange);
    }
}