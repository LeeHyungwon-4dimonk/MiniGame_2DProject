using DesignPattern;
using UnityEngine;

public class NormalMonsterSummoner : MonoBehaviour
{
    [field: SerializeField] public MonsterData Data {  get; private set; }
    [SerializeField] NormalMonsterController m_mob;
    
    private ObjectPool m_monsterPool;
    

    private void Awake()
    {
        m_monsterPool = new ObjectPool(transform, m_mob, 1);
    }

    private void OnEnable()
    {
        PooledObject monster = m_monsterPool.PopPool() as NormalMonsterController;
        monster.transform.position = transform.position;
    }
}
