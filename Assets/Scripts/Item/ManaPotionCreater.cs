using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotionCreater : PotionCreater
{
    private int m_status;

    private void Awake() => Init();

    private void Init()
    {
        m_status = Random.Range(1, 3);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance.GetCurHP() == GameManager.Instance.GetMaxHP()) return;
            Recover();
            gameObject.SetActive(false);
        }
    }

    public override void Recover()
    {
        GameManager.Instance.RecoverHp(m_status);
    }
}
