using DesignPattern;
using UnityEngine;

/// <summary>
/// 플레이어 스펠 발사 Muzzle
/// * 스펠 속도 조정시 Spell Speed 인스펙터로 조정
/// </summary>
public class PlayerMuzzle : MonoBehaviour
{
    // 스펠 프리팹
    [SerializeField] Spell m_spellPrefab;

    // 스펠 발사 속도(밸런싱)
    [SerializeField] float m_spellSpeed;
    
    // 플레이어 컴포넌트
    private PlayerController m_playerController;
    private SpriteRenderer m_playerSpriteRenderer;

    // 스펠 프리팹 컴포넌트
    private Rigidbody2D m_spellRigid;
    private SpriteRenderer m_spellSpriteRenderer;

    // 스펠 풀 생성
    private ObjectPool m_spellPool;

    private void Awake()
    {
        m_playerController = GetComponentInParent<PlayerController>();
        m_playerSpriteRenderer = GetComponentInParent<SpriteRenderer>();
        m_spellPool = new ObjectPool(transform, m_spellPrefab);
    }

    public void Fire()
    {
        // 플레이어가 차징 상태이면
        if (m_playerController.StateMach.CurState == m_playerController.StateMach.StateDic[EState.RangedAttack])
        {
            PooledObject spell = m_spellPool.PopPool() as Spell;
            m_spellRigid = spell.GetComponent<Rigidbody2D>();
            m_spellSpriteRenderer = spell.GetComponent<SpriteRenderer>();

            // 플레이어가 보는 방향 판정
            // 보는 방향에 따라 Muzzle의 위치를 이동시키고 발사 방향 결정 및 스펠 애니메이션 출력 방향 결정
            if (m_playerSpriteRenderer.flipX == false)
            {
                gameObject.transform.localPosition = new Vector3(0.8f, 0.7f, 0);
                spell.transform.position = gameObject.transform.position;
                m_spellRigid.AddForce(Vector2.right * m_spellSpeed, ForceMode2D.Impulse);
                m_spellSpriteRenderer.flipX = false;
            }
            else
            {
                gameObject.transform.localPosition = new Vector3(-0.8f, 0.7f, 0);
                spell.transform.position = gameObject.transform.position;
                m_spellRigid.AddForce(Vector2.left * m_spellSpeed, ForceMode2D.Impulse);
                m_spellSpriteRenderer.flipX = true;
            }            
        }
    }
}