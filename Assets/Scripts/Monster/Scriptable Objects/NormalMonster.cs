using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Normal Monster", menuName = "Scriptable Objects/Normal Monster", order = 1)]
public class NormalMonster : MonsterData
{
    public override void Attack(PlayerController controller)
    {
        controller.TakeDamage(MonsterAtk);
    }
}
