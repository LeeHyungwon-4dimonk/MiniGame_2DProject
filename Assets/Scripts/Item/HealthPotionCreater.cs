using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotionCreater : PotionCreater
{
    private int m_status;

    private void Awake() => Init();

    private void Init()
    {
        m_status = Random.Range(1, 4);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Recover();
            gameObject.SetActive(false);
        }
    }

    public override void Recover()
    {
        GameManager.Instance.RecoverHp(m_status);
    }
}
