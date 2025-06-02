using System.Collections;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamageable
{
    // 상자의 내구도
    [SerializeField] private int m_durability;

    // 상자가 부서질 때 드랍할 아이템
    [SerializeField] private GameObject[] m_gameObjects;

    private SFXController m_sfxController;

    private void Awake() => Init();

    private void Init()
    {
        m_sfxController = GetComponent<SFXController>();
    }

    public void TakeDamage(int damage)
    {
        m_durability -= damage;
        m_sfxController.PlaySFX("Damage");
        if(m_durability < 0 )
        { 
            m_durability = 0;
            Break();
        }
    }

    public void Break()
    {
        for(int i = 0; i < m_gameObjects.Length; i++)
        {
            Debug.Log("생성!");
            Instantiate(m_gameObjects[i], transform.position, Quaternion.identity);            
        }
        gameObject.SetActive(false);
    }
}
