using DesignPattern;
using UnityEngine;

public class BossMonsterMuzzle : MonoBehaviour
{
    // 스펠 프리팹
    [SerializeField] private MonsterSpell m_spellPrefab;

    // 보스 몬스터 스크립터블 오브젝트
    [SerializeField] private BossMonsterData m_bossMobData;

    // 보스 몬스터 컴포넌트
    private SpriteRenderer m_bossSpriteRenderer;

    // 스펠 프리팹 컴포넌트
    private Rigidbody2D m_spellRigid;
    private SpriteRenderer m_spellSpriteRenderer;

    // 스펠 풀 생성
    private ObjectPool m_spellPool;

    private void Awake()
    {
        m_bossSpriteRenderer = GetComponentInParent<SpriteRenderer>();
        m_spellPool = new ObjectPool(transform, m_spellPrefab);
    }

    public void Fire()
    {
        PooledObject spell = m_spellPool.PopPool() as MonsterSpell;
        m_spellRigid = spell.GetComponent<Rigidbody2D>();
        m_spellSpriteRenderer = spell.GetComponent<SpriteRenderer>();

        // 몬스터가 보는 방향 판정
        // 보는 방향에 따라 Muzzle의 위치를 이동시키고 발사 방향 결정 및 스펠 애니메이션 출력 방향 결정
        if (m_bossSpriteRenderer.flipX == true)
        {
            gameObject.transform.localPosition = new Vector3(4f, 1.2f, 0);
            spell.transform.position = gameObject.transform.position;
            m_spellRigid.AddForce(Vector2.right * m_bossMobData.SpellSpeed, ForceMode2D.Impulse);
            m_spellSpriteRenderer.flipX = false;
        }
        else
        {
            gameObject.transform.localPosition = new Vector3(-4f, 1.2f, 0);
            spell.transform.position = gameObject.transform.position;
            m_spellRigid.AddForce(Vector2.left * m_bossMobData.SpellSpeed, ForceMode2D.Impulse);
            m_spellSpriteRenderer.flipX = true;
        }
    }
}
